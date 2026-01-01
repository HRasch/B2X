# EventStorming Guide

## Übersicht

EventStorming ist eine kollaborative Workshop-Technik für Domain-Driven Design (DDD), die Teams dabei hilft, komplexe Geschäftsdomänen zu verstehen und zu modellieren. Durch die Fokussierung auf Domain Events werden verborgene Annahmen aufgedeckt und ein gemeinsames Verständnis der Geschäftsprozesse entwickelt.

**Zuletzt aktualisiert:** Januar 2026  
**Geeignet für:** B2Connect-Projekt, komplexe Domänen, Microservices-Architektur  
**Dauer:** 2-4 Stunden für Big Picture EventStorming

## Was ist EventStorming?

EventStorming ist eine Moderationstechnik, die von Alberto Brandolini entwickelt wurde. Sie kombiniert Elemente aus Event Modeling, Domain Storytelling und Design Thinking, um komplexe Geschäftssysteme zu analysieren.

**Kernprinzipien:**
- **Event-First Thinking:** Beginne mit dem, was im Geschäft passiert (Events), nicht mit dem, was das System tun soll
- **Kollaborative Modellierung:** Alle Stakeholder (Entwickler, Domain-Experten, Product Owner) arbeiten zusammen
- **Visuelle Sprache:** Verwende einfache Symbole und Farben für schnelle Kommunikation
- **Iterative Entdeckung:** Baue das Modell schrittweise auf, teste Annahmen

## Warum EventStorming im B2Connect-Projekt?

**Vorteile für B2Connect:**
- **Komplexe E-Commerce Domäne:** B2Connect hat komplexe Geschäftsprozesse (Bestellungen, Zahlungen, Versand, Retouren)
- **Microservices-Architektur:** Hilft bei der Identifizierung von Service-Grenzen und Event-getriebenen Architekturen
- **Cross-Functional Teams:** Fördert Zusammenarbeit zwischen Business und IT
- **Wolverine Integration:** EventStorming hilft bei der Definition von Commands und Events für Wolverine-Handler

**Wann EventStorming einsetzen:**
- Neue Feature-Entwicklung mit unbekannten Domänen
- Refactoring von Legacy-Systemen
- Definition von Microservice-Grenzen
- Verbesserung der Kommunikation zwischen Teams

## Vorbereitung

### Materialien
- **Große Papierrollen oder Whiteboard-Fläche** (mind. 10m Länge)
- **Klebezettel in verschiedenen Farben:**
  - Orange: Domain Events
  - Blau: Commands
  - Lila: Policies/Regeln
  - Gelb: User Stories/Fragen
  - Grün: Aggregates
  - Rosa: externe Systeme
  - Rot: Hotspots/Probleme
- **Marker in verschiedenen Farben**
- **Timer für Timeboxing**
- **Kamera für Dokumentation**

### Teilnehmer
- **Domain-Experten** (Product Owner, Business Analysten)
- **Entwickler** (Backend, Frontend, DevOps)
- **Architekten** (für technische Entscheidungen)
- **UX/UI Designer** (für User-Experience Perspektive)
- **Moderator** (erfahrener Facilitator)

**Gruppengröße:** 6-12 Personen optimal

### Raum-Setup
- Stehtische oder Whiteboard-Wände
- Genug Platz zum Bewegen
- Gute Beleuchtung für Fotos
- Technische Ausstattung für Remote-Teilnehmer (falls hybrid)

## Die EventStorming-Arten

### 1. Big Picture EventStorming
**Ziel:** Gesamtbild der Domäne verstehen  
**Dauer:** 2-4 Stunden  
**Fokus:** Alle wichtigen Events in chronologischer Reihenfolge

### 2. Process Level EventStorming
**Ziel:** Spezifische Geschäftsprozesse detailliert analysieren  
**Dauer:** 1-2 Tage  
**Fokus:** Einzelne Prozesse oder User Journeys

### 3. Design Level EventStorming
**Ziel:** Technische Implementierung planen  
**Dauer:** Halbtags-Workshops  
**Fokus:** Commands, Aggregates, Read Models

## Schritt-für-Schritt Anleitung

### Schritt 1: Domain Events sammeln (60-90 Minuten)

**Ziel:** Identifiziere alle wichtigen Ereignisse in der Domäne

**Vorgehen:**
1. **Start mit einem Trigger-Event:** "Ein Kunde hat erfolgreich bestellt"
2. **Brainstorming:** Jeder Teilnehmer schreibt Domain Events auf orangene Klebezettel
3. **Clustering:** Gruppiere ähnliche Events zusammen
4. **Timeline erstellen:** Ordne Events chronologisch an

**Beispiele für B2Connect:**
```
🧡 Customer Registered
🧡 Product Added to Cart
🧡 Order Placed
🧡 Payment Authorized
🧡 Order Confirmed
🧡 Shipment Prepared
🧡 Package Shipped
🧡 Delivery Confirmed
🧡 Return Requested
🧡 Refund Processed
```

**Tipps:**
- Verwende Vergangenheitsform für Events ("Order Placed" nicht "Place Order")
- Fokussiere auf Geschäftswert, nicht technische Details
- Stelle Fragen: "Was ist das nächste wichtige Ereignis?"

### Schritt 2: Commands identifizieren (30-45 Minuten)

**Ziel:** Finde heraus, was die Events auslöst

**Vorgehen:**
1. **Für jedes Event:** Frage "Was hat dieses Event verursacht?"
2. **Commands auf blaue Zettel schreiben**
3. **Pfeile zeichnen:** Command → Event

**Beispiele für B2Connect:**
```
💙 Place Order → 🧡 Order Placed
💙 Authorize Payment → 🧡 Payment Authorized
💙 Confirm Shipment → 🧡 Package Shipped
💙 Request Return → 🧡 Return Requested
```

### Schritt 3: Policies und Regeln hinzufügen (30 Minuten)

**Ziel:** Geschäftsregeln und automatische Reaktionen identifizieren

**Vorgehen:**
1. **Suche nach Mustern:** "Immer wenn X passiert, dann Y"
2. **Policies auf lila Zettel schreiben**
3. **Verbindungen zeichnen:** Event → Policy → Command

**Beispiele:**
```
💜 When Payment Authorized → Confirm Order
💜 When Order Confirmed → Prepare Shipment
💜 When Package Delivered → Send Satisfaction Survey
```

### Schritt 4: Aggregates definieren (30 Minuten)

**Ziel:** Konsistenzgrenzen und Verantwortlichkeiten identifizieren

**Vorgehen:**
1. **Gruppiere Commands und Events** um gemeinsame Entitäten
2. **Aggregates auf grüne Zettel schreiben**
3. **Consistency Boundaries zeichnen**

**Beispiele für B2Connect:**
```
💚 Order Aggregate: Place Order, Cancel Order, Update Order
💚 Customer Aggregate: Register Customer, Update Profile, Add Address
💚 Product Aggregate: Update Inventory, Change Price, Add Review
```

### Schritt 5: Externe Systeme und Akteure (20 Minuten)

**Ziel:** Schnittstellen zu anderen Systemen identifizieren

**Vorgehen:**
1. **Externe Systeme** auf rosa Zettel (Payment Provider, Shipping Service)
2. **User/Personas** auf separate Zettel
3. **API Calls** oder Integrationen markieren

### Schritt 6: Hotspots und Fragen identifizieren (20 Minuten)

**Ziel:** Unsicherheiten und Risiken aufdecken

**Vorgehen:**
1. **Gelbe Zettel** für offene Fragen und Annahmen
2. **Rote Zettel** für bekannte Probleme oder Risiken
3. **Diskussion** der kritischen Punkte

## Artefakte und Symbole

### Farbcodierung
- 🧡 **Orange:** Domain Events (Was ist passiert?)
- 💙 **Blau:** Commands (Was soll getan werden?)
- 💜 **Lila:** Policies/Regeln (Automatische Reaktionen)
- 💚 **Grün:** Aggregates (Konsistenzgrenzen)
- 🧡 **Gelb:** Fragen/Annahmen
- ❤️ **Rot:** Probleme/Risiken
- 🩷 **Rosa:** Externe Systeme

### Zusätzliche Symbole
- **→** Pfeile für kausale Verbindungen
- **□** Rechtecke für Aggregates
- **○** Kreise für wichtige Konzepte
- **!!** Ausrufezeichen für wichtige Events

## Moderationstechniken

### Timeboxing
- **5 Minuten** für individuelle Brainstorming-Phasen
- **2 Minuten** pro Person für Präsentation
- **10 Minuten** für Diskussion kontroverser Punkte

### Fragetechniken
- **Was passiert als nächstes?** (Event-Flow)
- **Wer oder was löst das aus?** (Command-Identifikation)
- **Gibt es Regeln dafür?** (Policy-Discovery)
- **Wo liegt die Verantwortung?** (Aggregate-Definition)

### Konfliktlösung
- **Punkteabstimmung** bei Uneinigkeit
- **Verschiedene Perspektiven** einbeziehen
- **Experimente vorschlagen** für unsichere Bereiche

## Nachbereitung

### Dokumentation
1. **Fotos** von der finalen EventStorming-Wand
2. **Digitale Kopie** mit Tools wie Miro oder Mural
3. **Zusammenfassung** der wichtigsten Erkenntnisse
4. **Action Items** mit Verantwortlichen und Deadlines

### Follow-up Aktivitäten
1. **Design Level EventStorming** für kritische Bereiche
2. **Code Prototyping** basierend auf identifizierten Aggregates
3. **API Design** für Service-Grenzen
4. **Test-Szenarien** aus den Event-Flows ableiten

## B2Connect-spezifische Beispiele

### E-Commerce Order Flow
```
Customer browses products → Product Selected
Customer adds to cart → Item Added to Cart
Customer proceeds to checkout → Checkout Started
Customer enters shipping info → Shipping Address Provided
Customer selects payment → Payment Method Selected
Customer places order → Order Placed
Payment system processes → Payment Authorized
Inventory checked → Inventory Reserved
Order confirmed → Order Confirmed
```

### Wolverine Integration
```csharp
// Commands aus EventStorming
public record PlaceOrder(
    Guid CustomerId,
    List<OrderItem> Items,
    Address ShippingAddress,
    PaymentInfo Payment);

// Events aus EventStorming
public record OrderPlaced(Guid OrderId, Guid CustomerId, decimal Total);
public record PaymentAuthorized(Guid OrderId, Guid PaymentId);
public record InventoryReserved(Guid OrderId, List<OrderItem> Items);

// Wolverine Handler
public class OrderPlacementHandler
{
    [Transactional]
    public async Task<OrderResult> Handle(PlaceOrder command)
    {
        // Business Logic basierend auf EventStorming
        var order = await _orderService.CreateOrder(command);
        await _eventPublisher.Publish(new OrderPlaced(order.Id, command.CustomerId, order.Total));
        return new OrderResult(order.Id);
    }
}
```

### Microservice-Grenzen
Basierend auf EventStorming können Service-Grenzen definiert werden:
- **Order Service:** Order Aggregate
- **Payment Service:** Payment Aggregate
- **Inventory Service:** Product/Inventory Aggregate
- **Shipping Service:** Shipment Aggregate

## Häufige Fallstricke

### Zu technisch denken
**Problem:** Teams springen zu früh zu technischen Lösungen  
**Lösung:** Fokussiere zuerst auf Business-Events, dann auf technische Implementierung

### Ungleiche Beteiligung
**Problem:** Einige Teilnehmer dominieren die Diskussion  
**Lösung:** Moderierte Runden, anonyme Zettel-Abstimmungen

### Zu detailliert werden
**Problem:** Teams verlieren sich in Mikro-Details  
**Lösung:** Big Picture zuerst, dann Process Level für wichtige Bereiche

### Fehlende Follow-up
**Problem:** EventStorming-Ergebnisse werden nicht verwendet  
**Lösung:** Sofort Action Items definieren und regelmäßig reviewen

## Tools und Ressourcen

### Digitale Tools
- **Miro:** Online-Whiteboarding für Remote-Sessions
- **Mural:** Kollaborative Workshop-Plattform
- **Microsoft Whiteboard:** Für Teams-Integration
- **Draw.io:** Für die digitale Nachbearbeitung

### Bücher und Artikel
- **Introducing EventStorming** von Alberto Brandolini
- **Domain-Driven Design** von Eric Evans
- **Event Modeling** von Adam Dymitruk

### Online-Ressourcen
- [EventStorming Website](https://www.eventstorming.com/)
- [DDD Community](https://dddcommunity.org/)
- [Alberto Brandolini's Blog](https://ziobrando.blogspot.com/)

## Metriken für Erfolg

### Quantitative Metriken
- **Event-Anzahl:** Mindestens 20-30 Domain Events für komplexe Domänen
- **Command-Event Ratio:** ~1:2 (mehr Events als Commands)
- **Teilnehmer-Engagement:** Alle sollten mindestens 5 Zettel beigetragen haben

### Qualitative Metriken
- **Gemeinsames Verständnis:** Alle Teilnehmer können den Prozess erklären
- **Neue Erkenntnisse:** Mindestens 3-5 "Aha"-Momente
- **Actionable Results:** Klare nächste Schritte definiert

## Integration mit B2Connect-Prozessen

### Vor EventStorming
- Domain-Experten identifizieren
- Zeitplan mit Stakeholdern abstimmen
- Materialien vorbereiten

### Nach EventStorming
- Ergebnisse in `.ai/decisions/` dokumentieren
- @Architect für Architecture Decision Records (ADRs)
- @ProductOwner für User Stories und Requirements
- @Backend für Wolverine-Implementierung

### Regelmäßige Reviews
- **Monatlich:** EventStorming-Ergebnisse reviewen
- **Pro Sprint:** Neue Erkenntnisse integrieren
- **Pro Release:** Big Picture EventStorming für neue Features

---

*EventStorming ist ein mächtiges Werkzeug für das B2Connect-Projekt, um komplexe E-Commerce-Domänen zu verstehen und robuste, Event-getriebene Microservices-Architekturen zu entwickeln.*