#!/bin/bash

# B2Connect Kubernetes Setup Script
# Vorbereitet den Kubernetes-Cluster für B2Connect Deployment

set -e

# Farben für Output
BLUE='\033[0;34m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Konfiguration
NAMESPACE="b2connect"
RELEASE_NAME="b2connect"
CHART_PATH="./backend/kubernetes/helm"
REGISTRY="${REGISTRY:-docker.io}"
REGISTRY_SECRET="${REGISTRY_SECRET:-regcred}"

# Hilfsfunktionen
log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Prüfe ob kubectl installiert ist
check_kubectl() {
    if ! command -v kubectl &> /dev/null; then
        log_error "kubectl nicht installiert. Bitte kubectl installieren."
        exit 1
    fi
    log_success "kubectl gefunden: $(kubectl version --client --short)"
}

# Prüfe ob helm installiert ist
check_helm() {
    if ! command -v helm &> /dev/null; then
        log_error "Helm nicht installiert. Bitte Helm installieren."
        exit 1
    fi
    log_success "Helm gefunden: $(helm version --short)"
}

# Prüfe Cluster-Verbindung
check_cluster() {
    log_info "Prüfe Cluster-Verbindung..."
    if kubectl cluster-info &> /dev/null; then
        CLUSTER_INFO=$(kubectl cluster-info | head -1)
        log_success "Mit Cluster verbunden: $CLUSTER_INFO"
    else
        log_error "Keine Verbindung zum Cluster. Bitte kubeconfig prüfen."
        exit 1
    fi
}

# Erstelle Namespace
create_namespace() {
    log_info "Erstelle Namespace '$NAMESPACE'..."
    
    if kubectl get namespace "$NAMESPACE" 2>/dev/null; then
        log_warning "Namespace '$NAMESPACE' existiert bereits"
    else
        kubectl create namespace "$NAMESPACE"
        log_success "Namespace '$NAMESPACE' erstellt"
    fi
    
    # Label für Service Discovery
    kubectl label namespace "$NAMESPACE" \
        name="$NAMESPACE" \
        --overwrite 2>/dev/null
}

# Erstelle Docker Registry Secret
create_registry_secret() {
    if [ -z "$REGISTRY_USERNAME" ] || [ -z "$REGISTRY_PASSWORD" ]; then
        log_warning "Registry-Credentials nicht gesetzt. Skipping Docker Registry Secret."
        return
    fi
    
    log_info "Erstelle Docker Registry Secret..."
    
    kubectl create secret docker-registry "$REGISTRY_SECRET" \
        --docker-server="$REGISTRY" \
        --docker-username="$REGISTRY_USERNAME" \
        --docker-password="$REGISTRY_PASSWORD" \
        --docker-email="admin@b2connect.local" \
        --namespace="$NAMESPACE" \
        --dry-run=client \
        -o yaml | kubectl apply -f -
    
    log_success "Docker Registry Secret erstellt"
}

# Erstelle Database Secrets
create_db_secrets() {
    log_info "Erstelle Database Secrets..."
    
    # Prüfe ob Secrets bereits existieren
    if kubectl get secret db-secrets -n "$NAMESPACE" 2>/dev/null; then
        log_warning "Secrets existieren bereits. Überschreibe..."
        kubectl delete secret db-secrets -n "$NAMESPACE"
    fi
    
    # Generiere sichere Passwörter, falls nicht gesetzt
    POSTGRES_PASSWORD="${POSTGRES_PASSWORD:-$(openssl rand -base64 32)}"
    
    # Verbindungsstrings mit Passwort
    AUTH_CONNECTION_STRING="Server=postgres;Port=5432;Database=b2connect_auth;User Id=b2connect;Password=$POSTGRES_PASSWORD;"
    TENANT_CONNECTION_STRING="Server=postgres;Port=5432;Database=b2connect_tenant;User Id=b2connect;Password=$POSTGRES_PASSWORD;"
    LOCALIZATION_CONNECTION_STRING="Server=postgres;Port=5432;Database=b2connect_localization;User Id=b2connect;Password=$POSTGRES_PASSWORD;"
    
    # Erstelle Secret
    kubectl create secret generic db-secrets \
        --from-literal=postgres-password="$POSTGRES_PASSWORD" \
        --from-literal=auth-connection-string="$AUTH_CONNECTION_STRING" \
        --from-literal=tenant-connection-string="$TENANT_CONNECTION_STRING" \
        --from-literal=localization-connection-string="$LOCALIZATION_CONNECTION_STRING" \
        --namespace="$NAMESPACE"
    
    log_success "Database Secrets erstellt"
    echo ""
    echo "🔐 Generierte Passwörter:"
    echo "   PostgreSQL Password: $POSTGRES_PASSWORD"
    echo ""
}

# Erstelle Storage Classes
create_storage_classes() {
    log_info "Prüfe Storage Classes..."
    
    if ! kubectl get storageclass standard 2>/dev/null; then
        log_warning "Standard Storage Class nicht gefunden"
        log_info "Erstelle lokale Storage Class..."
        
        cat << 'EOF' | kubectl apply -f -
apiVersion: storage.k8s.io/v1
kind: StorageClass
metadata:
  name: standard
provisioner: kubernetes.io/no-provisioner
volumeBindingMode: WaitForFirstConsumer
---
apiVersion: storage.k8s.io/v1
kind: StorageClass
metadata:
  name: fast
provisioner: kubernetes.io/no-provisioner
volumeBindingMode: WaitForFirstConsumer
EOF
        log_success "Storage Classes erstellt"
    else
        log_success "Storage Classes existieren bereits"
    fi
}

# Installiere RBAC
install_rbac() {
    log_info "Installiere RBAC..."
    
    # ServiceAccount
    kubectl apply -f - << EOF
apiVersion: v1
kind: ServiceAccount
metadata:
  name: apphost
  namespace: $NAMESPACE
EOF
    
    # ClusterRole
    kubectl apply -f - << EOF
apiVersion: rbac.authorization.k8s.io/v1
kind: ClusterRole
metadata:
  name: apphost-reader
rules:
- apiGroups: [""]
  resources: ["services", "endpoints", "pods"]
  verbs: ["get", "list", "watch"]
- apiGroups: ["apps"]
  resources: ["deployments", "statefulsets", "daemonsets"]
  verbs: ["get", "list", "watch"]
EOF
    
    # ClusterRoleBinding
    kubectl apply -f - << EOF
apiVersion: rbac.authorization.k8s.io/v1
kind: ClusterRoleBinding
metadata:
  name: apphost-reader-binding
roleRef:
  apiGroup: rbac.authorization.k8s.io
  kind: ClusterRole
  name: apphost-reader
subjects:
- kind: ServiceAccount
  name: apphost
  namespace: $NAMESPACE
EOF
    
    log_success "RBAC installiert"
}

# Validiere Helm Chart
validate_chart() {
    log_info "Validiere Helm Chart..."
    
    if [ ! -d "$CHART_PATH" ]; then
        log_error "Chart nicht gefunden: $CHART_PATH"
        exit 1
    fi
    
    helm lint "$CHART_PATH"
    log_success "Chart validiert"
}

# Installiere oder upgrade Helm Release
deploy_helm_release() {
    log_info "Deployment Helm Release..."
    
    if helm list -n "$NAMESPACE" | grep -q "$RELEASE_NAME"; then
        log_info "Upgrade bestehende Release..."
        helm upgrade "$RELEASE_NAME" "$CHART_PATH" \
            --namespace "$NAMESPACE" \
            --values "$CHART_PATH/values.yaml"
    else
        log_info "Installiere neue Release..."
        helm install "$RELEASE_NAME" "$CHART_PATH" \
            --namespace "$NAMESPACE" \
            --values "$CHART_PATH/values.yaml"
    fi
    
    log_success "Helm Release deployed"
}

# Warte auf Deployments
wait_for_deployments() {
    log_info "Warte auf Deployments (max 5 Minuten)..."
    
    kubectl rollout status deployment -n "$NAMESPACE" --timeout=300s
    
    log_success "Alle Deployments bereit"
}

# Zeige Deployment-Status
show_status() {
    echo ""
    echo -e "${BLUE}=== B2Connect Deployment Status ===${NC}"
    echo ""
    
    log_info "Namespace: $NAMESPACE"
    kubectl get namespace "$NAMESPACE"
    echo ""
    
    log_info "Pods:"
    kubectl get pods -n "$NAMESPACE"
    echo ""
    
    log_info "Services:"
    kubectl get services -n "$NAMESPACE"
    echo ""
    
    log_info "Secrets:"
    kubectl get secrets -n "$NAMESPACE"
    echo ""
    
    log_info "PersistentVolumeClaims:"
    kubectl get pvc -n "$NAMESPACE"
    echo ""
    
    log_success "Setup abgeschlossen!"
}

# Port-Forwarding für AppHost
setup_port_forwarding() {
    log_info "Richte Port-Forwarding ein für AppHost..."
    
    echo ""
    echo "Um auf AppHost zuzugreifen, führe folgendes aus:"
    echo "  kubectl port-forward -n $NAMESPACE svc/apphost 9000:9000"
    echo ""
    echo "Danach unter http://localhost:9000 erreichbar"
    echo ""
}

# Hauptausführung
main() {
    echo ""
    echo -e "${BLUE}╔════════════════════════════════════════════╗${NC}"
    echo -e "${BLUE}║  B2Connect Kubernetes Setup Script        ║${NC}"
    echo -e "${BLUE}║  Aspire 10 - Microservices Hosting       ║${NC}"
    echo -e "${BLUE}╚════════════════════════════════════════════╝${NC}"
    echo ""
    
    # Prüfe Voraussetzungen
    check_kubectl
    check_helm
    check_cluster
    
    echo ""
    
    # Erstelle Infrastruktur
    create_namespace
    create_registry_secret
    create_db_secrets
    create_storage_classes
    install_rbac
    
    echo ""
    
    # Deployment
    validate_chart
    deploy_helm_release
    
    echo ""
    
    # Warte auf Readiness
    wait_for_deployments
    
    echo ""
    
    # Zeige Status
    show_status
    setup_port_forwarding
    
    log_success "B2Connect Kubernetes Setup abgeschlossen!"
}

# Starte wenn direkt aufgerufen
if [ "${BASH_SOURCE[0]}" == "${0}" ]; then
    main "$@"
fi
