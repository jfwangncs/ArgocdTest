# ArgoTest Helm Chart

这是 ArgoTest 应用的 Helm Chart，支持 Argo Rollouts 金丝雀部署策略。

## 安装

### 基本安装

```bash
helm install argotest ./helm -n argotest --create-namespace
```

### 使用自定义值安装

```bash
helm install argotest ./helm -n argotest --create-namespace -f custom-values.yaml
```

## 升级

```bash
helm upgrade argotest ./helm -n argotest
```

## 卸载

```bash
helm uninstall argotest -n argotest
```

## 配置

主要配置参数在 `values.yaml` 中：

| 参数                 | 描述             | 默认值                    |
| -------------------- | ---------------- | ------------------------- |
| `namespace.name`     | 命名空间名称     | `argotest`                |
| `namespace.create`   | 是否创建命名空间 | `true`                    |
| `image.repository`   | 镜像仓库         | `jfwangncs/argotest`      |
| `image.tag`          | 镜像标签         | `v5`                      |
| `image.pullPolicy`   | 镜像拉取策略     | `Always`                  |
| `replicaCount`       | 副本数量         | `2`                       |
| `service.port`       | 服务端口         | `80`                      |
| `service.targetPort` | 容器端口         | `8080`                    |
| `ingress.enabled`    | 是否启用 Ingress | `true`                    |
| `ingress.host`       | Ingress 主机名   | `argotest.xiaofengyu.com` |

## 金丝雀发布策略

Chart 默认配置了金丝雀发布策略：

1. **20%流量** - 需要手动确认
2. **40%流量** - 自动等待 3 分钟
3. **60%流量** - 自动等待 3 分钟
4. **80%流量** - 自动等待 3 分钟
5. **100%流量** - 完成发布

## 示例：自定义配置

创建 `custom-values.yaml`:

```yaml
image:
  tag: v6

replicaCount: 3

ingress:
  host: myapp.example.com

resources:
  limits:
    cpu: 500m
    memory: 512Mi
  requests:
    cpu: 250m
    memory: 256Mi
```

然后使用：

```bash
helm upgrade argotest ./helm -n argotest -f custom-values.yaml
```

## 注意事项

- 需要先安装 Argo Rollouts Controller
- 需要先安装 NGINX Ingress Controller
- 如果启用 TLS，需要先安装 cert-manager
