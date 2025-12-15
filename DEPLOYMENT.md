# ArgoCD + Argo Rollouts 部署指南

## 前置要求

1. Kubernetes 集群
2. kubectl 配置完成
3. 域名 argotest.xiaofengyu.com 已解析到集群
4. Docker Hub 账号并已推送镜像

## 部署步骤

### 1. 安装 ArgoCD

```bash
kubectl create namespace argocd
kubectl apply -n argocd -f https://raw.githubusercontent.com/argoproj/argo-cd/stable/manifests/install.yaml
```

获取 ArgoCD 管理员密码：

```bash
kubectl -n argocd get secret argocd-initial-admin-secret -o jsonpath="{.data.password}" | base64 -d
```

访问 ArgoCD UI：

```bash
kubectl port-forward svc/argocd-server -n argocd 8080:443
```

访问 https://localhost:8080，用户名: admin

### 2. 安装 Argo Rollouts

```bash
kubectl create namespace argo-rollouts
kubectl apply -n argo-rollouts -f https://github.com/argoproj/argo-rollouts/releases/latest/download/install.yaml
```

安装 kubectl rollouts 插件：

```bash
# Linux/Mac
curl -LO https://github.com/argoproj/argo-rollouts/releases/latest/download/kubectl-argo-rollouts-linux-amd64
chmod +x ./kubectl-argo-rollouts-linux-amd64
sudo mv ./kubectl-argo-rollouts-linux-amd64 /usr/local/bin/kubectl-argo-rollouts

# Windows (使用 PowerShell)
Invoke-WebRequest -Uri "https://github.com/argoproj/argo-rollouts/releases/latest/download/kubectl-argo-rollouts-windows-amd64.exe" -OutFile kubectl-argo-rollouts.exe
Move-Item kubectl-argo-rollouts.exe $HOME\bin\kubectl-argo-rollouts.exe
```

### 3. 安装 NGINX Ingress Controller

```bash
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.8.1/deploy/static/provider/cloud/deploy.yaml
```

### 4. 配置 TLS（选择一种方式）

#### 方式 A：使用 cert-manager 自动获取证书（推荐）

```bash
# 安装 cert-manager
kubectl apply -f https://github.com/cert-manager/cert-manager/releases/download/v1.13.0/cert-manager.yaml

# 创建 Let's Encrypt Issuer
cat <<EOF | kubectl apply -f -
apiVersion: cert-manager.io/v1
kind: ClusterIssuer
metadata:
  name: letsencrypt-prod
spec:
  acme:
    server: https://acme-v02.api.letsencrypt.org/directory
    email: your-email@example.com  # 替换为你的邮箱
    privateKeySecretRef:
      name: letsencrypt-prod
    solvers:
    - http01:
        ingress:
          class: nginx
EOF
```

#### 方式 B：使用现有证书

```bash
kubectl create secret tls argotest-tls \
  --cert=path/to/tls.crt \
  --key=path/to/tls.key \
  -n argotest
```

### 5. 修改配置文件

**修改 k8s/rollout.yaml：**

- 将 `你的用户名/argotest:latest` 替换为你的 Docker Hub 镜像

**修改 argocd/application.yaml：**

- 将 `https://github.com/你的用户名/ArgocdTest.git` 替换为你的 Git 仓库地址

### 6. 提交代码到 Git 仓库

```bash
git add .
git commit -m "Add k8s and argocd configurations"
git push origin master
```

### 7. 部署 ArgoCD Application

```bash
kubectl apply -f argocd/application.yaml
```

### 8. 查看部署状态

```bash
# 查看 ArgoCD 应用状态
kubectl get applications -n argocd

# 查看 Rollout 状态
kubectl argo rollouts get rollout argotest -n argotest

# 实时监控 Rollout
kubectl argo rollouts get rollout argotest -n argotest --watch

# 查看 Rollout Dashboard
kubectl argo rollouts dashboard
```

## 金丝雀部署流程

部署流程会按以下步骤自动进行：

1. **20% 流量** → 暂停，等待手动确认
2. **手动确认后** → 40% 流量 → 等待 3 分钟
3. **自动** → 60% 流量 → 等待 3 分钟
4. **自动** → 80% 流量 → 等待 3 分钟
5. **自动** → 100% 流量（完成部署）

### 手动确认命令

当部署暂停在 20% 时，使用以下命令继续：

```bash
kubectl argo rollouts promote argotest -n argotest
```

### 回滚命令

如果需要回滚到上一个版本：

```bash
kubectl argo rollouts abort argotest -n argotest
kubectl argo rollouts undo argotest -n argotest
```

## 更新部署

修改 [k8s/rollout.yaml](k8s/rollout.yaml) 中的镜像标签，提交到 Git：

```bash
# 例如更新到新版本
# image: 你的用户名/argotest:2

git add k8s/rollout.yaml
git commit -m "Update to version 2"
git push origin master
```

ArgoCD 会自动检测变化并开始金丝雀部署。

## 访问应用

```
https://argotest.xiaofengyu.com
```

## 监控命令

```bash
# 查看 Pod 状态
kubectl get pods -n argotest

# 查看 Service
kubectl get svc -n argotest

# 查看 Ingress
kubectl get ingress -n argotest

# 查看日志
kubectl logs -f deployment/argotest -n argotest

# 查看 Rollout 详情
kubectl describe rollout argotest -n argotest
```

## 故障排查

### Rollout 无法启动

```bash
kubectl describe rollout argotest -n argotest
kubectl logs -n argo-rollouts deployment/argo-rollouts
```

### Ingress 无法访问

```bash
kubectl describe ingress argotest-ingress -n argotest
kubectl logs -n ingress-nginx deployment/ingress-nginx-controller
```

### TLS 证书问题

```bash
kubectl describe certificate argotest-tls -n argotest
kubectl describe certificaterequest -n argotest
```
