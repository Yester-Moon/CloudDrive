# CloudDrive API 文档

> 基础路径：`/api`  
> 认证方式：JWT Bearer Token（在请求头中添加 `Authorization: Bearer <token>`）  
> 统一响应格式见 [通用响应结构](#通用响应结构)

---

## 目录

- [通用响应结构](#通用响应结构)
- [认证模块 Auth](#认证模块-auth)
  - [用户注册](#用户注册)
  - [用户登录](#用户登录)
  - [刷新 Token](#刷新-token)
  - [用户登出](#用户登出)
- [文件模块 Files](#文件模块-files)
  - [上传文件](#上传文件)
  - [分片上传（秒传检测）](#分片上传秒传检测)
  - [下载文件](#下载文件)
  - [获取文件列表（分页）](#获取文件列表分页)
  - [获取文件详情](#获取文件详情)
  - [删除文件](#删除文件)
  - [重命名文件](#重命名文件)
  - [移动文件](#移动文件)
  - [复制文件](#复制文件)
  - [搜索文件](#搜索文件)
  - [创建文件夹](#创建文件夹)
- [分享模块 Share](#分享模块-share)
  - [创建分享链接](#创建分享链接)
  - [获取分享信息（公开）](#获取分享信息公开)
  - [验证分享密码](#验证分享密码)
  - [通过分享链接下载文件](#通过分享链接下载文件)
  - [取消分享](#取消分享)
  - [获取我的分享列表](#获取我的分享列表)
  - [获取分享统计](#获取分享统计)
- [用户模块 User](#用户模块-user)
  - [获取当前用户信息](#获取当前用户信息)
  - [更新用户信息](#更新用户信息)
  - [修改密码](#修改密码)
  - [获取配额信息](#获取配额信息)
  - [获取用户统计信息](#获取用户统计信息)

---

## 通用响应结构

所有接口（除文件下载外）均返回以下 JSON 结构：

```json
{
  "success": true,
  "message": "操作成功",
  "data": null,
  "code": 200,
  "timestamp": "2024-01-01T00:00:00"
}
```

| 字段 | 类型 | 说明 |
|------|------|------|
| `success` | `boolean` | 请求是否成功 |
| `message` | `string` | 提示信息 |
| `data` | `object/null` | 响应数据 |
| `code` | `int` | 状态码（200/400/404/409/500） |
| `timestamp` | `datetime` | 响应时间戳 |

---

## 认证模块 Auth

### 用户注册

```
POST /api/auth/register
```

**认证**：无需认证

**请求体**：

| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `userName` | `string` | ✅ | 用户名 |
| `email` | `string` | ✅ | 邮箱 |
| `password` | `string` | ✅ | 密码 |

**请求示例**：

```json
{
  "userName": "testuser",
  "email": "test@example.com",
  "password": "P@ssw0rd123"
}
```

**响应**：`200 OK`

```json
{
  "success": true,
  "message": "注册成功",
  "data": { /* 用户信息 */ },
  "code": 200
}
```

---

### 用户登录

```
POST /api/auth/login
```

**认证**：无需认证

**请求体**：

| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `userName` | `string` | ✅ | 用户名 |
| `password` | `string` | ✅ | 密码 |

**请求示例**：

```json
{
  "userName": "testuser",
  "password": "P@ssw0rd123"
}
```

**响应**：`200 OK`

```json
{
  "success": true,
  "message": "登录成功",
  "data": {
    "user": { /* 用户信息 */ },
    "token": "eyJhbGciOiJIUzI1NiIs..."
  },
  "code": 200
}
```

---

### 刷新 Token

```
POST /api/auth/refresh
```

**认证**：🔒 需要 Bearer Token

**请求体**：无

**响应**：`200 OK`

```json
{
  "success": true,
  "message": "Token刷新成功",
  "data": {
    "user": { /* 用户信息 */ }
  },
  "code": 200
}
```

**错误响应**：

| 状态码 | 说明 |
|--------|------|
| `404` | 用户不存在 |

---

### 用户登出

```
POST /api/auth/logout
```

**认证**：🔒 需要 Bearer Token

**请求体**：无

**响应**：`200 OK`

```json
{
  "success": true,
  "message": "登出成功",
  "code": 200
}
```

> **说明**：JWT 为无状态认证，登出由客户端删除 Token 实现。

---

## 文件模块 Files

> 本模块所有接口均需要 Bearer Token 认证（`[Authorize]`）。

### 上传文件

```
POST /api/files/upload
```

**Content-Type**：`multipart/form-data`  
**大小限制**：最大 10GB

**请求参数**：

| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `file` | `file` | ✅ | 上传的文件 |
| `parentFolderId` | `guid` | ❌ | 父文件夹 ID（为空时上传到根目录） |
| `fileHash` | `string` | ❌ | 文件哈希值（用于秒传检测） |

**响应**：`200 OK`

```json
{
  "success": true,
  "message": "上传成功",
  "data": {
    "success": true,
    "fileId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "fileName": "example.pdf",
    "fileSize": 1048576,
    "isInstantUpload": false,
    "isDuplicate": false,
    "errorMessage": null
  },
  "code": 200
}
```

**错误响应**：

| 状态码 | 说明 |
|--------|------|
| `400` | 上传失败 |
| `409` | 文件重复 |

---

### 分片上传（秒传检测）

```
POST /api/files/upload/chunk
```

**请求体**：

| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `fileHash` | `string` | ✅ | 文件哈希值 |
| `fileName` | `string` | ✅ | 文件名 |
| `mimeType` | `string` | ✅ | MIME 类型 |
| `parentFolderId` | `guid` | ❌ | 父文件夹 ID |

**请求示例**：

```json
{
  "fileHash": "d41d8cd98f00b204e9800998ecf8427e",
  "fileName": "example.pdf",
  "mimeType": "application/pdf",
  "parentFolderId": null
}
```

**响应（秒传成功）**：`200 OK`

```json
{
  "success": true,
  "message": "秒传成功",
  "data": { /* FileUploadResultDto */ },
  "code": 200
}
```

**响应（需正常上传）**：`200 OK`

```json
{
  "success": false,
  "message": "文件需要正常上传",
  "code": 400
}
```

**错误响应**：

| 状态码 | 说明 |
|--------|------|
| `409` | 文件重复 |

---

### 下载文件

```
GET /api/files/{id}/download
```

**路径参数**：

| 字段 | 类型 | 说明 |
|------|------|------|
| `id` | `guid` | 文件 ID |

**响应**：文件流（`Content-Type` 为对应 MIME 类型，`Content-Disposition` 包含文件名）

---

### 获取文件列表（分页）

```
GET /api/files
```

**查询参数**：

| 字段 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `parentFolderId` | `guid` | `null` | 父文件夹 ID（为空时查询根目录） |
| `pageIndex` | `int` | `1` | 页码（从 1 开始） |
| `pageSize` | `int` | `20` | 每页数量 |
| `sortBy` | `string` | `null` | 排序字段 |
| `ascending` | `bool` | `true` | 是否升序 |

**响应**：`200 OK`

```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "example.pdf",
        "extension": ".pdf",
        "size": 1048576,
        "formattedSize": "1 MB",
        "mimeType": "application/pdf",
        "isFolder": false,
        "parentFolderId": null,
        "hash": "d41d8cd98f00b204e9800998ecf8427e",
        "downloadCount": 0,
        "tags": null,
        "description": null,
        "thumbnailUrl": null,
        "creationTime": "2024-01-01T00:00:00",
        "lastModificationTime": null
      }
    ],
    "totalCount": 1,
    "pageIndex": 1,
    "pageSize": 20,
    "totalPages": 1,
    "hasPreviousPage": false,
    "hasNextPage": false,
    "currentFolderId": null,
    "currentFolderName": null
  },
  "code": 200
}
```

---

### 获取文件详情

```
GET /api/files/{id}
```

**路径参数**：

| 字段 | 类型 | 说明 |
|------|------|------|
| `id` | `guid` | 文件 ID |

**响应**：`200 OK`

```json
{
  "success": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "example.pdf",
    "extension": ".pdf",
    "size": 1048576,
    "formattedSize": "1 MB",
    "mimeType": "application/pdf",
    "isFolder": false,
    "parentFolderId": null,
    "hash": "d41d8cd98f00b204e9800998ecf8427e",
    "downloadCount": 0,
    "tags": null,
    "description": null,
    "thumbnailUrl": null,
    "creationTime": "2024-01-01T00:00:00",
    "lastModificationTime": null
  },
  "code": 200
}
```

**错误响应**：

| 状态码 | 说明 |
|--------|------|
| `404` | 文件不存在 |

---

### 删除文件

```
DELETE /api/files/{id}
```

**路径参数**：

| 字段 | 类型 | 说明 |
|------|------|------|
| `id` | `guid` | 文件 ID |

**响应**：`200 OK`

```json
{
  "success": true,
  "message": "删除成功",
  "code": 200
}
```

---

### 重命名文件

```
PUT /api/files/{id}
```

**路径参数**：

| 字段 | 类型 | 说明 |
|------|------|------|
| `id` | `guid` | 文件 ID |

**请求体**：

| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `newName` | `string` | ✅ | 新文件名 |

**请求示例**：

```json
{
  "newName": "新文件名.pdf"
}
```

**响应**：`200 OK`

```json
{
  "success": true,
  "message": "重命名成功",
  "data": { /* FileInfoDto */ },
  "code": 200
}
```

---

### 移动文件

```
POST /api/files/{id}/move
```

**路径参数**：

| 字段 | 类型 | 说明 |
|------|------|------|
| `id` | `guid` | 文件 ID |

**请求体**：

| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `targetFolderId` | `guid` | ❌ | 目标文件夹 ID（为空移动到根目录） |

**请求示例**：

```json
{
  "targetFolderId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

**响应**：`200 OK`

```json
{
  "success": true,
  "message": "移动成功",
  "code": 200
}
```

---

### 复制文件

```
POST /api/files/{id}/copy
```

**路径参数**：

| 字段 | 类型 | 说明 |
|------|------|------|
| `id` | `guid` | 文件 ID |

**请求体**：

| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `targetFolderId` | `guid` | ❌ | 目标文件夹 ID（为空复制到根目录） |

**请求示例**：

```json
{
  "targetFolderId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

**响应**：`200 OK`

```json
{
  "success": true,
  "message": "复制成功",
  "data": { /* FileInfoDto */ },
  "code": 200
}
```

---

### 搜索文件

```
GET /api/files/search
```

**查询参数**：

| 字段 | 类型 | 必填 | 默认值 | 说明 |
|------|------|------|--------|------|
| `keyword` | `string` | ✅ | - | 搜索关键词 |
| `pageIndex` | `int` | ❌ | `1` | 页码 |
| `pageSize` | `int` | ❌ | `20` | 每页数量 |

**响应**：`200 OK`

```json
{
  "success": true,
  "data": {
    "items": [ /* FileInfoDto[] */ ],
    "totalCount": 0,
    "pageIndex": 1,
    "pageSize": 20,
    "totalPages": 0,
    "hasPreviousPage": false,
    "hasNextPage": false
  },
  "code": 200
}
```

---

### 创建文件夹

```
POST /api/files/folder
```

**请求体**：

| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `folderName` | `string` | ✅ | 文件夹名称 |
| `parentFolderId` | `guid` | ❌ | 父文件夹 ID（为空创建在根目录） |

**请求示例**：

```json
{
  "folderName": "新建文件夹",
  "parentFolderId": null
}
```

**响应**：`200 OK`

```json
{
  "success": true,
  "message": "文件夹创建成功",
  "data": { /* FileInfoDto */ },
  "code": 200
}
```

---

## 分享模块 Share

### 创建分享链接

```
POST /api/share
```

**认证**：🔒 需要 Bearer Token

**请求体**：

| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `fileItemId` | `guid` | ✅ | 被分享的文件 ID |
| `title` | `string` | ❌ | 分享标题 |
| `accessPassword` | `string` | ❌ | 访问密码（为空则无需密码） |
| `expirationTime` | `datetime` | ❌ | 过期时间（为空则永不过期） |
| `maxDownloadCount` | `int` | ❌ | 最大下载次数（为空则不限） |
| `allowDownload` | `bool` | ❌ | 是否允许下载（默认 `true`） |

**请求示例**：

```json
{
  "fileItemId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "title": "项目文档分享",
  "accessPassword": "1234",
  "expirationTime": "2024-12-31T23:59:59",
  "maxDownloadCount": 10,
  "allowDownload": true
}
```

**响应**：`200 OK`

```json
{
  "success": true,
  "message": "分享创建成功",
  "data": {
    "id": "...",
    "shareCode": "abc123",
    "title": "项目文档分享",
    "fileItemId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "fileName": "document.pdf",
    "fileSize": 1048576,
    "hasPassword": true,
    "expirationTime": "2024-12-31T23:59:59",
    "maxDownloadCount": 10,
    "currentDownloadCount": 0,
    "remainingDownloadCount": 10,
    "viewCount": 0,
    "allowDownload": true,
    "status": "有效",
    "isValid": true,
    "creationTime": "2024-01-01T00:00:00"
  },
  "code": 200
}
```

---

### 获取分享信息（公开）

```
GET /api/share/{code}
```

**认证**：无需认证

**路径参数**：

| 字段 | 类型 | 说明 |
|------|------|------|
| `code` | `string` | 分享码 |

**响应**：`200 OK`

```json
{
  "success": true,
  "data": { /* ShareLinkDto */ },
  "code": 200
}
```

**错误响应**：

| 状态码 | 说明 |
|--------|------|
| `404` | 分享链接不存在 |

---

### 验证分享密码

```
POST /api/share/{code}/verify
```

**认证**：无需认证

**路径参数**：

| 字段 | 类型 | 说明 |
|------|------|------|
| `code` | `string` | 分享码 |

**请求体**：

| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `password` | `string` | ❌ | 访问密码 |

**请求示例**：

```json
{
  "password": "1234"
}
```

**响应**：`200 OK`

```json
{
  "success": true,
  "message": "验证成功",
  "data": { /* FileInfoDto */ },
  "code": 200
}
```

**错误响应**：

| 状态码 | 说明 |
|--------|------|
| `404` | 文件不存在 |

---

### 通过分享链接下载文件

```
GET /api/share/{code}/download
```

**认证**：无需认证

**路径参数**：

| 字段 | 类型 | 说明 |
|------|------|------|
| `code` | `string` | 分享码 |

**查询参数**：

| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `password` | `string` | ❌ | 访问密码（有密码时必填） |

**响应**：文件流（`Content-Type` 为对应 MIME 类型，`Content-Disposition` 包含文件名）

---

### 取消分享

```
DELETE /api/share/{id}
```

**认证**：🔒 需要 Bearer Token

**路径参数**：

| 字段 | 类型 | 说明 |
|------|------|------|
| `id` | `guid` | 分享链接 ID |

**响应**：`200 OK`

```json
{
  "success": true,
  "message": "分享已取消",
  "code": 200
}
```

---

### 获取我的分享列表

```
GET /api/share
```

**认证**：🔒 需要 Bearer Token

**响应**：`200 OK`

```json
{
  "success": true,
  "data": [ /* ShareLinkDto[] */ ],
  "code": 200
}
```

---

### 获取分享统计

```
GET /api/share/{id}/stats
```

**认证**：🔒 需要 Bearer Token

**路径参数**：

| 字段 | 类型 | 说明 |
|------|------|------|
| `id` | `guid` | 分享链接 ID |

**响应**：`200 OK`

```json
{
  "success": true,
  "data": { /* ShareLinkDto（含统计信息） */ },
  "code": 200
}
```

**错误响应**：

| 状态码 | 说明 |
|--------|------|
| `404` | 分享链接不存在 |

---

## 用户模块 User

> 本模块所有接口均需要 Bearer Token 认证（`[Authorize]`）。

### 获取当前用户信息

```
GET /api/user/profile
```

**响应**：`200 OK`

```json
{
  "success": true,
  "data": { /* 用户信息 */ },
  "code": 200
}
```

**错误响应**：

| 状态码 | 说明 |
|--------|------|
| `404` | 用户不存在 |

---

### 更新用户信息

```
PUT /api/user/profile
```

**请求体**：

| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `displayName` | `string` | ❌ | 显示名称 |
| `avatarUrl` | `string` | ❌ | 头像 URL |

**请求示例**：

```json
{
  "displayName": "新昵称",
  "avatarUrl": "https://example.com/avatar.png"
}
```

**响应**：`200 OK`

```json
{
  "success": true,
  "message": "更新成功",
  "data": { /* 用户信息 */ },
  "code": 200
}
```

---

### 修改密码

```
POST /api/user/password
```

**请求体**：

| 字段 | 类型 | 必填 | 说明 |
|------|------|------|------|
| `currentPassword` | `string` | ✅ | 当前密码 |
| `newPassword` | `string` | ✅ | 新密码 |

**请求示例**：

```json
{
  "currentPassword": "OldP@ss123",
  "newPassword": "NewP@ss456"
}
```

**响应**：`200 OK`

```json
{
  "success": true,
  "message": "密码修改成功",
  "code": 200
}
```

---

### 获取配额信息

```
GET /api/user/quota
```

**响应**：`200 OK`

```json
{
  "success": true,
  "data": { /* 配额信息 */ },
  "code": 200
}
```

---

### 获取用户统计信息

```
GET /api/user/statistics
```

**响应**：`200 OK`

```json
{
  "success": true,
  "data": {
    "quota": { /* 配额信息 */ },
    "totalFiles": 100,
    "totalShares": 10,
    "activeShares": 5,
    "totalDownloads": 200,
    "totalViews": 500
  },
  "code": 200
}
```

---

## 错误码说明

| 状态码 | 说明 |
|--------|------|
| `200` | 请求成功 |
| `400` | 请求参数错误 / 业务逻辑错误 |
| `401` | 未认证（缺少或无效的 Token） |
| `404` | 资源不存在 |
| `409` | 资源冲突（如文件重复） |
| `500` | 服务器内部错误 |
