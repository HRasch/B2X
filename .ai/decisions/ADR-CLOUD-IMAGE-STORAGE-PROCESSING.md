# ADR: Cloud-Based Image Storage with Dynamic Processing

**DocID**: `ADR-CLOUD-IMAGE`  
**Status**: DRAFT  
**Date**: 2026-01-01  
**Author**: @Architect  
**Reviewers**: @Backend, @Frontend, @DevOps, @Security, @TechLead

---

## Context

B2Connect requires a robust image storage and processing solution to support:
- Product images (multiple sizes, formats)
- CMS media assets
- User-generated content (UGC)
- Dynamic image transformations (resize, crop, format conversion)
- Modern format delivery (WebP, AVIF)
- CDN integration for global delivery
- Visual search capabilities (future: image embeddings)

Current state: Basic file upload to local/cloud storage without transformation pipeline.

**Requirements from Product Vision** (see [PRODUCT_VISION.md](../.ai/requirements/PRODUCT_VISION.md)):
> "Media & image delivery: support serving images from cloud blob storage (S3, Azure Blob, GCS), first‑class support for modern formats (WebP, AVIF), and dynamic on‑the‑fly resizing/optimization with CDN integration."

---

## Decision Drivers

| Priority | Driver |
|----------|--------|
| P0 | Performance: Sub-100ms image delivery globally |
| P0 | Modern formats: Automatic WebP/AVIF conversion |
| P1 | Self-hosted option: Control over data & costs |
| P1 | Integration: Works with existing S3/Azure/GCS storage |
| P2 | Cost predictability: Avoid surprise bills at scale |
| P2 | Security: Signed URLs, access control |
| P3 | AI features: Future support for image embeddings/visual search |

---

## Options Considered

### Option 1: imgproxy (Self-Hosted) ⭐ RECOMMENDED

**Overview**: Self-hosted, Go-based image processing server. Open-core model with Pro features.

| Aspect | Details |
|--------|---------|
| **Type** | Self-hosted (Docker/K8s) |
| **License** | Open Source (MIT) + Pro ($49/mo) |
| **Performance** | Fastest in benchmarks (Go + libvips) |
| **Formats** | JPEG, PNG, GIF, WebP, AVIF, JPEG XL, HEIC |
| **Storage** | Any HTTP source, S3, GCS, Azure Blob |
| **Security** | URL signing, authorization headers |
| **Pricing** | Infrastructure cost only (self-hosted) |

**Pros**:
- ✅ **Fastest performance** - Go-based, highly optimized
- ✅ **No vendor lock-in** - runs on your infrastructure
- ✅ **Predictable costs** - no per-request pricing
- ✅ **URL-based API** - simple integration
- ✅ **Modern format support** - AVIF, WebP, JPEG XL
- ✅ **Security built-in** - URL signing, image bomb protection
- ✅ **Kubernetes-native** - easy scaling

**Cons**:
- ❌ Infrastructure management required
- ❌ Pro features require license ($49/mo or $499/yr)
- ❌ No built-in DAM/asset management

**Pro Features** (paid):
- Video thumbnail generation
- Advanced watermarking
- PDF/PSD preview generation
- GIF to MP4 conversion
- Advanced smart cropping (object detection)

**URL Example**:
```
https://imgproxy.example.com/{signature}/rs:fill:300:200/g:sm/plain/s3://bucket/image.jpg
```

**References**:
- Website: https://imgproxy.net/
- GitHub: https://github.com/imgproxy/imgproxy (10.2k stars)
- Docs: https://docs.imgproxy.net/

---

### Option 2: Imageflow (Self-Hosted)

**Overview**: Rust-based image processing library with .NET bindings. Created by ImageResizer team.

| Aspect | Details |
|--------|---------|
| **Type** | Self-hosted (library/.NET Server) |
| **License** | AGPLv3 (free) or Commercial |
| **Performance** | Up to 17x faster than ImageMagick |
| **Formats** | JPEG, PNG, GIF, WebP, AVIF |
| **Storage** | S3, Azure Blob, filesystem |
| **Security** | URL signing |
| **Pricing** | AGPLv3 free / Commercial license |

**Pros**:
- ✅ **Excellent .NET integration** - Imageflow.NET Server
- ✅ **High quality output** - linear light processing
- ✅ **Fast** - Rust core, native performance
- ✅ **Query string API** - ImageResizer compatible
- ✅ **Mature** - 10+ years of development

**Cons**:
- ❌ AGPLv3 requires open-sourcing or commercial license
- ❌ Smaller community than imgproxy
- ❌ Less active development recently
- ❌ No built-in object detection

**URL Example**:
```
/images/photo.jpg?width=300&height=200&mode=crop&format=webp
```

**References**:
- Website: https://www.imageflow.io/
- GitHub: https://github.com/imazen/imageflow (4.4k stars)
- .NET Server: https://github.com/imazen/imageflow-dotnet-server

---

### Option 3: Thumbor (Self-Hosted)

**Overview**: Python-based smart image service with AI cropping.

| Aspect | Details |
|--------|---------|
| **Type** | Self-hosted (Python/Docker) |
| **License** | MIT (fully open source) |
| **Performance** | Moderate (Python-based) |
| **Formats** | JPEG, PNG, GIF, WebP |
| **Storage** | S3, filesystem, HTTP |
| **Security** | URL signing |
| **Pricing** | Free |

**Pros**:
- ✅ **100% open source** - no paid tiers
- ✅ **Smart cropping** - face/feature detection
- ✅ **Extensible** - plugin architecture
- ✅ **Large community** - well documented

**Cons**:
- ❌ **Slower** than Go/Rust alternatives
- ❌ **No AVIF support** by default
- ❌ **Python dependency** - different from our stack
- ❌ **Less active** - fewer updates recently

**URL Example**:
```
/unsafe/300x200/smart/example.com/image.jpg
```

**References**:
- Website: https://thumbor.org/
- GitHub: https://github.com/thumbor/thumbor (9.6k stars)

---

### Option 4: Cloudinary (SaaS)

**Overview**: Full-featured media management SaaS with AI capabilities.

| Aspect | Details |
|--------|---------|
| **Type** | SaaS (managed) |
| **License** | Proprietary |
| **Performance** | Excellent (global CDN) |
| **Formats** | All formats + video |
| **Storage** | Cloudinary-managed |
| **Security** | Signed URLs, access control |
| **Pricing** | Usage-based (can be expensive) |

**Pros**:
- ✅ **Zero infrastructure** - fully managed
- ✅ **Feature-rich** - AI background removal, auto-tagging
- ✅ **Global CDN** - built-in delivery
- ✅ **DAM included** - asset management
- ✅ **.NET SDK** - good integration
- ✅ **Video support** - streaming, transcoding

**Cons**:
- ❌ **Expensive at scale** - usage-based pricing
- ❌ **Vendor lock-in** - proprietary storage
- ❌ **Data residency** - limited control
- ❌ **Unpredictable costs** - traffic spikes = surprise bills

**Pricing** (2025):
- Free: 25 credits/month
- Plus: $99/month (225 credits)
- Advanced: $249/month (600 credits)
- Enterprise: Custom

**URL Example**:
```
https://res.cloudinary.com/demo/image/upload/w_300,h_200,c_fill/sample.jpg
```

**References**:
- Website: https://cloudinary.com/
- Docs: https://cloudinary.com/documentation

---

### Option 5: ImageKit (SaaS)

**Overview**: Modern image/video API platform with DAM.

| Aspect | Details |
|--------|---------|
| **Type** | SaaS with external storage support |
| **License** | Proprietary |
| **Performance** | Excellent (CloudFront CDN) |
| **Formats** | All modern formats + video |
| **Storage** | Your S3/GCS/Azure + ImageKit DAM |
| **Security** | Signed URLs, access control |
| **Pricing** | Usage-based (more predictable) |

**Pros**:
- ✅ **Use existing storage** - no migration needed
- ✅ **Modern formats** - WebP, AVIF automatic
- ✅ **Video streaming** - HLS/DASH support
- ✅ **AI features** - smart crop, background removal
- ✅ **Better pricing** than Cloudinary
- ✅ **ISO 27001, GDPR compliant**

**Cons**:
- ❌ **Still SaaS** - some vendor dependency
- ❌ **Usage-based** - costs scale with traffic
- ❌ **Less feature-rich** than Cloudinary

**Pricing** (2025):
- Free: 20GB bandwidth/month
- Standard: From $49/month
- Enterprise: Custom

**URL Example**:
```
https://ik.imagekit.io/your_id/image.jpg?tr=w-300,h-200
```

**References**:
- Website: https://imagekit.io/
- Docs: https://imagekit.io/docs/

---

### Option 6: Bunny Optimizer (SaaS/CDN)

**Overview**: CDN with integrated image optimization.

| Aspect | Details |
|--------|---------|
| **Type** | CDN + Optimization |
| **License** | Proprietary |
| **Performance** | Excellent (global PoPs) |
| **Formats** | WebP, AVIF |
| **Storage** | Origin-based (your storage) |
| **Security** | Token auth |
| **Pricing** | Fixed $9.50/month per site |

**Pros**:
- ✅ **Simple pricing** - flat rate per site
- ✅ **No per-request cost** - unlimited optimizations
- ✅ **Built-in CDN** - global delivery
- ✅ **Easy setup** - no code changes

**Cons**:
- ❌ **Limited transformations** - basic resize/crop
- ❌ **No advanced AI features**
- ❌ **Less flexible** than dedicated solutions

**References**:
- Website: https://bunny.net/optimizer/

---

## Comparison Matrix

| Feature | imgproxy | Imageflow | Thumbor | Cloudinary | ImageKit | Bunny |
|---------|----------|-----------|---------|------------|----------|-------|
| **Type** | Self-hosted | Self-hosted | Self-hosted | SaaS | SaaS | CDN+SaaS |
| **Performance** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐⭐ |
| **WebP** | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| **AVIF** | ✅ | ✅ | ❌ | ✅ | ✅ | ✅ |
| **Smart Crop** | Pro | Basic | ✅ | ✅ | ✅ | Basic |
| **Video** | Pro | ❌ | ❌ | ✅ | ✅ | ❌ |
| **AI Features** | Pro | ❌ | Basic | ✅✅✅ | ✅✅ | ❌ |
| **DAM** | ❌ | ❌ | ❌ | ✅ | ✅ | ❌ |
| **.NET SDK** | ❌ | ✅✅✅ | ❌ | ✅ | ✅ | ❌ |
| **Use Own Storage** | ✅ | ✅ | ✅ | ❌ | ✅ | ✅ |
| **Predictable Cost** | ✅✅✅ | ✅✅✅ | ✅✅✅ | ❌ | ⭐⭐ | ✅✅✅ |
| **Vendor Lock-in** | None | None | None | High | Medium | Low |

---

## Recommendation

### Primary: **imgproxy** (Self-Hosted)

**Rationale**:
1. **Best performance** - Go-based, fastest in benchmarks
2. **Cost predictable** - infrastructure cost only, no per-request fees
3. **No vendor lock-in** - runs on our Kubernetes cluster
4. **Modern formats** - AVIF, WebP, JPEG XL support
5. **Security-first** - URL signing, image bomb protection
6. **Simple integration** - URL-based API, works with any storage

### Secondary (Fallback): **Imageflow** (.NET native)

**When to consider**:
- If deep .NET integration is critical
- If AGPLv3 is acceptable or commercial license acquired
- For specific ImageResizer compatibility needs

### Not Recommended for B2Connect:

| Option | Reason |
|--------|--------|
| Cloudinary | Too expensive at scale, vendor lock-in |
| Thumbor | Slower performance, no AVIF, Python dependency |
| Bunny | Limited transformation features |

---

## Proposed Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                         B2Connect                                │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  ┌──────────────┐     ┌──────────────┐     ┌──────────────┐     │
│  │   Frontend   │     │   Admin UI   │     │   Store API  │     │
│  └──────┬───────┘     └──────┬───────┘     └──────┬───────┘     │
│         │                    │                    │              │
│         └────────────────────┼────────────────────┘              │
│                              │                                   │
│                              ▼                                   │
│                    ┌─────────────────┐                          │
│                    │   CDN (Edge)    │                          │
│                    │  CloudFront/    │                          │
│                    │  Bunny/Fastly   │                          │
│                    └────────┬────────┘                          │
│                             │                                    │
│                             ▼                                    │
│                    ┌─────────────────┐                          │
│                    │    imgproxy     │                          │
│                    │  (Kubernetes)   │                          │
│                    │  - URL signing  │                          │
│                    │  - Resize/crop  │                          │
│                    │  - Format conv  │                          │
│                    └────────┬────────┘                          │
│                             │                                    │
│              ┌──────────────┼──────────────┐                    │
│              ▼              ▼              ▼                    │
│       ┌───────────┐  ┌───────────┐  ┌───────────┐              │
│       │    S3     │  │   Azure   │  │    GCS    │              │
│       │  Bucket   │  │   Blob    │  │  Bucket   │              │
│       └───────────┘  └───────────┘  └───────────┘              │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

### URL Structure

```
# Pattern
https://images.b2connect.io/{signature}/{processing}/{source}

# Examples
# Resize to 300x200, WebP format
https://images.b2connect.io/abc123/rs:fill:300:200/f:webp/plain/s3://products/shoe.jpg

# Smart crop, AVIF format
https://images.b2connect.io/abc123/rs:fill:400:400/g:sm/f:avif/plain/s3://products/model.jpg

# Watermarked product image
https://images.b2connect.io/abc123/rs:fit:800:600/wm:0.5:ce:10:10:0.3/plain/s3://products/item.jpg
```

### Kubernetes Deployment

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: imgproxy
  namespace: media
spec:
  replicas: 3
  selector:
    matchLabels:
      app: imgproxy
  template:
    metadata:
      labels:
        app: imgproxy
    spec:
      containers:
      - name: imgproxy
        image: darthsim/imgproxy:latest
        ports:
        - containerPort: 8080
        env:
        - name: IMGPROXY_KEY
          valueFrom:
            secretKeyRef:
              name: imgproxy-secrets
              key: key
        - name: IMGPROXY_SALT
          valueFrom:
            secretKeyRef:
              name: imgproxy-secrets
              key: salt
        - name: IMGPROXY_USE_S3
          value: "true"
        - name: AWS_ACCESS_KEY_ID
          valueFrom:
            secretKeyRef:
              name: aws-credentials
              key: access-key
        - name: AWS_SECRET_ACCESS_KEY
          valueFrom:
            secretKeyRef:
              name: aws-credentials
              key: secret-key
        - name: IMGPROXY_MAX_SRC_RESOLUTION
          value: "50"  # 50 megapixels max
        - name: IMGPROXY_ALLOWED_SOURCES
          value: "s3://b2connect-*"
        resources:
          requests:
            memory: "512Mi"
            cpu: "500m"
          limits:
            memory: "2Gi"
            cpu: "2000m"
```

---

## Implementation Plan

### Phase 1: Infrastructure Setup (Week 1-2)
- [ ] Deploy imgproxy to Kubernetes (dev environment)
- [ ] Configure S3/Azure Blob as source storage
- [ ] Set up URL signing with secure keys
- [ ] Configure CDN caching layer

### Phase 2: Backend Integration (Week 2-3)
- [ ] Create `IImageService` abstraction
- [ ] Implement `ImgproxyImageService`
- [ ] Add URL generation helpers
- [ ] Integrate with Product entity (images)

### Phase 3: Frontend Integration (Week 3-4)
- [ ] Create Vue composable `useOptimizedImage`
- [ ] Add responsive image component
- [ ] Implement lazy loading with placeholders
- [ ] Add srcset generation

### Phase 4: CMS Integration (Week 4-5)
- [ ] Media upload API → S3
- [ ] Thumbnail generation on upload
- [ ] Image picker with transformation preview
- [ ] Bulk image optimization tool

### Phase 5: Production & Monitoring (Week 5-6)
- [ ] Production deployment (HA configuration)
- [ ] Monitoring & alerting setup
- [ ] Performance benchmarking
- [ ] Documentation & runbooks

---

## Cost Estimate

### Self-Hosted (imgproxy) - Monthly

| Resource | Specification | Cost (AWS) |
|----------|---------------|------------|
| imgproxy (3x pods) | 2 vCPU, 4GB RAM | ~$150 |
| S3 Storage | 500GB | ~$12 |
| CloudFront CDN | 1TB egress | ~$85 |
| **Total** | | **~$250/month** |

### SaaS Comparison (at 1TB/month egress)

| Provider | Monthly Cost |
|----------|--------------|
| imgproxy (self-hosted) | ~$250 |
| ImageKit | ~$200-400 |
| Cloudinary | ~$500-1000+ |
| Bunny Optimizer | ~$10 + CDN |

---

## Security Considerations

1. **URL Signing**: All transformation URLs must be signed
2. **Source Restriction**: Only allow configured S3 buckets
3. **Size Limits**: Max source resolution 50MP
4. **Rate Limiting**: Implement at CDN and imgproxy level
5. **CORS**: Configure allowed origins
6. **Secrets**: Store signing keys in Kubernetes secrets/Vault

---

## Risks & Mitigations

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|------------|
| imgproxy unavailable | Low | High | Multi-replica deployment, health checks |
| High processing load | Medium | Medium | Horizontal scaling, CDN caching |
| Security vulnerability | Low | High | Regular updates, image bomb protection |
| Storage costs spike | Medium | Low | Lifecycle policies, monitoring alerts |

---

## Decision

**Selected Option**: imgproxy (Self-Hosted)

**Approved by**:
- [ ] @Architect
- [ ] @TechLead
- [ ] @DevOps
- [ ] @Security

---

## References

- [imgproxy Documentation](https://docs.imgproxy.net/)
- [imgproxy GitHub](https://github.com/imgproxy/imgproxy)
- [Imageflow GitHub](https://github.com/imazen/imageflow)
- [ImageKit Documentation](https://imagekit.io/docs/)
- [Cloudinary Documentation](https://cloudinary.com/documentation)
- [B2Connect Product Vision](../.ai/requirements/PRODUCT_VISION.md)
- [B2Connect Technical Requirements](../.ai/requirements/TECHNICAL_REQUIREMENTS.md)

---

**Agents**: @Architect, @SARAH | Owner: @Architect
