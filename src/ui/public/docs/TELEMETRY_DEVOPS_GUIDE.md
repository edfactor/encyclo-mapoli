# Telemetry Configuration & Monitoring Guide for DevOps

## Production Deployment Checklist

### ✅ Pre-Deployment

- [ ] OpenTelemetry collector configured and accessible
- [ ] Sensitive field tracking disabled (`EnableSensitiveFieldTracking: false`)
- [ ] PII masking enabled (`PiiMaskingEnabled: true`)
- [ ] Sampling rate set appropriately (1-5% for production)
- [ ] Metric retention policies configured
- [ ] Log rotation policies in place
- [ ] Monitoring dashboards created
- [ ] Alerting rules deployed

### ✅ Configuration Files

**appsettings.Production.json**

```json
{
  "OpenTelemetry": {
    "Enabled": true,
    "Otlp": {
      "Endpoint": "https://otel-collector.company.com:4317"
    },
    "Tracing": {
      "Sampler": "parentbased_traceidratio",
      "SamplerRate": 0.02
    },
    "Metrics": {
      "Enabled": true
    }
  },
  "Telemetry": {
    "EnableSensitiveFieldTracking": false,
    "LargeResponseThresholdBytes": 5242880,
    "PiiMaskingEnabled": true
  }
}
```

**Environment Variables**

```bash
# Required
OTEL_ENABLED=true
OTEL_OTLP_ENDPOINT=https://otel-collector.company.com:4317

# Security (Critical)
TELEMETRY_ENABLE_SENSITIVE_FIELD_TRACKING=false
TELEMETRY_PII_MASKING_ENABLED=true

# Performance Tuning
OTEL_TRACING_SAMPLER_RATE=0.02
TELEMETRY_LARGE_RESPONSE_THRESHOLD_BYTES=5242880

# Resource Limits
OTEL_RESOURCE_ATTRIBUTES=service.name=profit-sharing-api,service.version=1.0.0
```

### ✅ Infrastructure Requirements

**OpenTelemetry Collector**

```yaml
# otel-collector-config.yaml
receivers:
  otlp:
    protocols:
      grpc:
        endpoint: 0.0.0.0:4317
      http:
        endpoint: 0.0.0.0:4318

processors:
  batch:
    timeout: 1s
    send_batch_size: 1024
  memory_limiter:
    limit_mib: 512

exporters:
  prometheus:
    endpoint: "0.0.0.0:8889"
  logging:
    loglevel: info

service:
  pipelines:
    metrics:
      receivers: [otlp]
      processors: [memory_limiter, batch]
      exporters: [prometheus, logging]
    traces:
      receivers: [otlp]
      processors: [memory_limiter, batch]
      exporters: [logging]
```

## Monitoring Setup

### Prometheus Configuration

**prometheus.yml**

```yaml
global:
  scrape_interval: 15s
  evaluation_interval: 15s

rule_files:
  - "profit_sharing_alerts.yml"

scrape_configs:
  - job_name: "profit-sharing-api"
    static_configs:
      - targets: ["otel-collector:8889"]
    metrics_path: /metrics
    scrape_interval: 30s
```

### Grafana Dashboard

Import the dashboard JSON:

```json
{
  "dashboard": {
    "id": null,
    "title": "Profit Sharing API - Production Monitoring",
    "tags": ["profit-sharing", "api", "production"],
    "panels": [
      {
        "id": 1,
        "title": "Request Rate",
        "type": "graph",
        "targets": [
          {
            "expr": "rate(app_requests_total[5m])",
            "legendFormat": "{{endpoint_category}} - {{user_role}}"
          }
        ],
        "yAxes": [
          {
            "label": "Requests/sec"
          }
        ]
      },
      {
        "id": 2,
        "title": "Error Rate %",
        "type": "stat",
        "targets": [
          {
            "expr": "rate(ps_endpoint_errors_total[5m]) / rate(app_requests_total[5m]) * 100",
            "legendFormat": "Error Rate"
          }
        ],
        "thresholds": [
          { "color": "green", "value": 0 },
          { "color": "yellow", "value": 1 },
          { "color": "red", "value": 5 }
        ]
      },
      {
        "id": 3,
        "title": "Response Time P95",
        "type": "graph",
        "targets": [
          {
            "expr": "histogram_quantile(0.95, rate(app_request_duration_seconds_bucket[5m]))",
            "legendFormat": "95th percentile"
          }
        ]
      },
      {
        "id": 4,
        "title": "Business Operations",
        "type": "table",
        "targets": [
          {
            "expr": "increase(ps_business_operations_total[1h])",
            "legendFormat": "{{operation}}"
          }
        ]
      },
      {
        "id": 5,
        "title": "Sensitive Data Access",
        "type": "stat",
        "targets": [
          {
            "expr": "increase(ps_sensitive_field_access_total{field=\"Ssn\"}[1h])",
            "legendFormat": "SSN Access/Hour"
          }
        ],
        "thresholds": [
          { "color": "green", "value": 0 },
          { "color": "yellow", "value": 50 },
          { "color": "red", "value": 100 }
        ]
      },
      {
        "id": 6,
        "title": "Large Responses",
        "type": "graph",
        "targets": [
          {
            "expr": "increase(ps_large_responses_total[10m])",
            "legendFormat": "{{endpoint_category}}"
          }
        ]
      }
    ]
  }
}
```

### Alert Rules

**profit_sharing_alerts.yml**

```yaml
groups:
  - name: profit_sharing_critical
    rules:
      - alert: HighErrorRate
        expr: rate(ps_endpoint_errors_total[5m]) / rate(app_requests_total[5m]) > 0.05
        for: 2m
        labels:
          severity: critical
          service: profit-sharing-api
        annotations:
          summary: "High error rate detected"
          description: "Error rate is {{ $value | humanizePercentage }} for the last 5 minutes"
          runbook: "https://wiki.company.com/runbooks/profit-sharing-errors"

      - alert: APIDown
        expr: up{job="profit-sharing-api"} == 0
        for: 1m
        labels:
          severity: critical
          service: profit-sharing-api
        annotations:
          summary: "Profit Sharing API is down"
          description: "The Profit Sharing API has been down for more than 1 minute"

  - name: profit_sharing_warning
    rules:
      - alert: HighResponseTime
        expr: histogram_quantile(0.95, rate(app_request_duration_seconds_bucket[5m])) > 10
        for: 5m
        labels:
          severity: warning
          service: profit-sharing-api
        annotations:
          summary: "High response time detected"
          description: "95th percentile response time is {{ $value }}s"

      - alert: ExcessiveLargeResponses
        expr: increase(ps_large_responses_total[10m]) > 10
        for: 1m
        labels:
          severity: warning
          service: profit-sharing-api
        annotations:
          summary: "Excessive large responses detected"
          description: "{{ $value }} large responses in the last 10 minutes"

  - name: profit_sharing_security
    rules:
      - alert: UnusualSSNAccess
        expr: rate(ps_sensitive_field_access_total{field="Ssn"}[1h]) > 100
        for: 5m
        labels:
          severity: warning
          service: profit-sharing-api
          team: security
        annotations:
          summary: "Unusual SSN access pattern detected"
          description: "SSN access rate is {{ $value }} per hour"
          action: "Review logs for correlation ID and user activity"

      - alert: SuspiciousDataExport
        expr: ps_large_responses_total{user_role!="ADMINISTRATOR"} > 5
        for: 1m
        labels:
          severity: critical
          service: profit-sharing-api
          team: security
        annotations:
          summary: "Suspicious large data export by non-admin"
          description: "Non-admin user generating large responses"
          action: "Immediately review user activity and consider account suspension"
```

## Log Management

### Log Aggregation Configuration

**Fluentd/Fluent Bit Configuration**

```yaml
[INPUT]
    Name              tail
    Path              /var/log/profit-sharing/*.log
    Parser            json
    Tag               profit-sharing.*
    Refresh_Interval  5

[FILTER]
    Name    grep
    Match   profit-sharing.*
    Regex   level (Information|Warning|Error|Critical)

[FILTER]
    Name    modify
    Match   profit-sharing.*
    Add     service profit-sharing-api
    Add     environment production

[OUTPUT]
    Name  elasticsearch
    Match profit-sharing.*
    Host  elasticsearch.company.com
    Port  9200
    Index profit-sharing-logs
    Type  _doc
```

### Log Retention Policies

```yaml
# Elasticsearch Index Lifecycle Management
PUT _ilm/policy/profit-sharing-logs-policy
{
  "policy": {
    "phases": {
      "hot": {
        "actions": {
          "rollover": {
            "max_size": "50gb",
            "max_age": "7d"
          }
        }
      },
      "warm": {
        "min_age": "7d",
        "actions": {
          "allocate": {
            "number_of_replicas": 0
          }
        }
      },
      "cold": {
        "min_age": "30d",
        "actions": {
          "allocate": {
            "number_of_replicas": 0
          }
        }
      },
      "delete": {
        "min_age": "90d"
      }
    }
  }
}
```

## Performance Tuning

### Application Configuration

**High Volume Endpoints**

```json
{
  "Telemetry": {
    "SamplingRules": {
      "YearEndReportEndpoint": 0.1, // 10% sampling
      "MasterInquiryEndpoint": 0.05, // 5% sampling
      "DefaultEndpoints": 0.02 // 2% sampling
    }
  }
}
```

### Resource Limits

**Kubernetes Deployment**

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: profit-sharing-api
spec:
  replicas: 3
  template:
    spec:
      containers:
        - name: api
          image: profit-sharing-api:latest
          resources:
            requests:
              memory: "512Mi"
              cpu: "250m"
            limits:
              memory: "1Gi"
              cpu: "500m"
          env:
            - name: OTEL_ENABLED
              value: "true"
            - name: OTEL_TRACING_SAMPLER_RATE
              value: "0.02"
            - name: TELEMETRY_ENABLE_SENSITIVE_FIELD_TRACKING
              value: "false"
```

### Database Impact Monitoring

Monitor for telemetry impact on database:

```sql
-- Check for telemetry-related performance impact
SELECT
    query_text,
    total_time,
    calls,
    mean_time
FROM pg_stat_statements
WHERE query_text LIKE '%telemetry%'
ORDER BY total_time DESC;
```

## Security Configuration

### Network Security

**Firewall Rules**

```bash
# Allow OTLP traffic to collector
iptables -A OUTPUT -d otel-collector.company.com -p tcp --dport 4317 -j ACCEPT
iptables -A OUTPUT -d otel-collector.company.com -p tcp --dport 4318 -j ACCEPT

# Block direct external telemetry endpoints
iptables -A OUTPUT -p tcp --dport 4317 -j DROP
iptables -A OUTPUT -p tcp --dport 4318 -j DROP
```

### TLS Configuration

**OTLP over TLS**

```json
{
  "OpenTelemetry": {
    "Otlp": {
      "Endpoint": "https://otel-collector.company.com:4317",
      "Headers": {
        "Authorization": "Bearer ${OTEL_AUTH_TOKEN}"
      },
      "Timeout": 30000
    }
  }
}
```

### Secrets Management

**Azure Key Vault Integration**

```csharp
// In Program.cs
builder.Configuration.AddAzureKeyVault(
    vaultUri: "https://your-vault.vault.azure.net/",
    credential: new DefaultAzureCredential());
```

**Environment Variables (Kubernetes)**

```yaml
env:
  - name: OTEL_AUTH_TOKEN
    valueFrom:
      secretKeyRef:
        name: telemetry-secrets
        key: otel-auth-token
```

## Capacity Planning

### Storage Requirements

**Metrics Storage (Prometheus)**

- ~1KB per metric sample
- ~100 metrics per request
- Retention: 30 days
- Formula: `requests_per_day * 100 * 1KB * 30 days`

**Log Storage (Elasticsearch)**

- ~500 bytes per log entry
- ~5 log entries per request
- Retention: 90 days
- Formula: `requests_per_day * 5 * 500B * 90 days`

**Example Calculation (10k requests/day)**

- Metrics: 10,000 _ 100 _ 1KB \* 30 = ~30GB
- Logs: 10,000 _ 5 _ 500B \* 90 = ~2.25GB

### Network Bandwidth

**OTLP Traffic Estimation**

- ~2KB per request (compressed)
- Peak hours consideration
- Network redundancy

### Monitoring Infrastructure Scaling

**Auto-scaling Rules**

```yaml
# Prometheus
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: prometheus-hpa
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: prometheus
  minReplicas: 2
  maxReplicas: 5
  metrics:
    - type: Resource
      resource:
        name: memory
        target:
          type: Utilization
          averageUtilization: 70
```

## Disaster Recovery

### Backup Strategy

**Prometheus Data**

```bash
# Automated backup script
#!/bin/bash
BACKUP_DIR="/backups/prometheus/$(date +%Y-%m-%d)"
mkdir -p $BACKUP_DIR

# Create snapshot
curl -X POST http://prometheus:9090/api/v1/admin/tsdb/snapshot

# Copy snapshot
cp -r /prometheus/snapshots/latest/* $BACKUP_DIR/

# Compress and upload
tar -czf $BACKUP_DIR.tar.gz $BACKUP_DIR
aws s3 cp $BACKUP_DIR.tar.gz s3://company-backups/prometheus/
```

**Grafana Dashboards**

```bash
# Export all dashboards
for dashboard in $(curl -s "http://admin:password@grafana:3000/api/search" | jq -r '.[].uri'); do
  curl -s "http://admin:password@grafana:3000/api/dashboards/$dashboard" | \
  jq '.dashboard' > "backup_$(echo $dashboard | sed 's/\//_/g').json"
done
```

### Recovery Procedures

**Telemetry Data Loss Recovery**

1. Check application logs for correlation IDs
2. Reconstruct critical metrics from application logs
3. Use database audit logs for business operations
4. Rebuild dashboards from backup

**Service Recovery Priority**

1. Application functionality (telemetry optional)
2. Critical alerts (error rate, downtime)
3. Business metrics
4. Detailed performance metrics

## Compliance & Auditing

### Audit Log Requirements

**What to Audit**

- Sensitive field access (who, when, what)
- Configuration changes
- Alert rule modifications
- Dashboard access patterns

**Audit Log Format**

```json
{
  "timestamp": "2025-09-25T10:30:00Z",
  "event_type": "sensitive_field_access",
  "user_id": "masked_user_123",
  "user_role": "ADMINISTRATOR",
  "field_accessed": "Ssn",
  "endpoint": "YearEndReportEndpoint",
  "correlation_id": "abc123-def456",
  "records_affected": 1,
  "masked_value": "***-**-6789"
}
```

### Retention Policies

**Production Environment**

- Metrics: 30 days detailed, 1 year aggregated
- Logs: 90 days detailed, 7 years audit logs
- Traces: 7 days detailed, 30 days sampled

**Compliance Requirements**

- SOX: 7 years financial transaction logs
- GDPR: PII processing logs with deletion capability
- HIPAA: If applicable, healthcare data access logs

### Data Privacy Controls

**PII Handling Checklist**

- [ ] All PII masked in telemetry
- [ ] Correlation IDs used instead of direct identifiers
- [ ] Data subject deletion procedures in place
- [ ] Cross-border data transfer compliance
- [ ] Third-party processor agreements

## Troubleshooting Runbook

### Common Issues

**1. Missing Metrics**

```bash
# Check OTLP connectivity
curl -v https://otel-collector.company.com:4317/health

# Verify application configuration
kubectl logs deployment/profit-sharing-api | grep -i telemetry

# Test metric generation
curl -X POST "https://api.company.com/test-endpoint" \
  -H "Authorization: Bearer $TOKEN"
```

**2. High Memory Usage**

```bash
# Check telemetry memory impact
ps aux | grep profit-sharing-api
grep -i telemetry /proc/$(pidof profit-sharing-api)/smaps

# Adjust sampling rate
kubectl set env deployment/profit-sharing-api OTEL_TRACING_SAMPLER_RATE=0.01
```

**3. Alert Fatigue**

```promql
# Review alert frequency
ALERTS{alertname!="", alertstate="firing"}

# Tune thresholds based on historical data
histogram_quantile(0.99, rate(app_request_duration_seconds_bucket[7d]))
```

### Emergency Procedures

**Complete Telemetry Shutdown**

```bash
# Disable telemetry without restart
kubectl set env deployment/profit-sharing-api OTEL_ENABLED=false

# Scale down monitoring
kubectl scale deployment prometheus --replicas=0
kubectl scale deployment grafana --replicas=0
```

**Rapid Recovery**

```bash
# Quick telemetry restore
kubectl set env deployment/profit-sharing-api OTEL_ENABLED=true
kubectl rollout restart deployment/profit-sharing-api

# Verify recovery
kubectl get pods -l app=profit-sharing-api
curl https://api.company.com/health
```

### Contact Information

**Escalation Path**

1. Development Team: dev-team@company.com
2. DevOps Team: devops@company.com
3. Security Team: security@company.com
4. Management: it-manager@company.com

**24/7 Support**

- PagerDuty: profit-sharing-api-alerts
- Slack: #profit-sharing-support
- Phone: +1-800-IT-SUPPORT

---

_This guide is maintained by the DevOps team. Last updated: September 25, 2025_
