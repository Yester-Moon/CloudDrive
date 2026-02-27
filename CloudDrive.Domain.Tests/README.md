# CloudDrive.Domain.Tests

## 📋 概述

这是 CloudDrive 领域层的单元测试项目，使用 xUnit 测试框架，配合 FluentAssertions 进行断言，以及 Moq 用于模拟依赖。

## 🧪 测试覆盖范围

### 1. 值对象测试 (ValueObjects)
- **FileSizeTests** - 测试文件大小值对象
  - 创建和相等性比较
  - 各种文件大小场景
  
- **FilePathTests** - 测试文件路径值对象
  - 路径创建和验证
  - 不同路径格式处理
  
- **FileHashTests** - 测试文件哈希值对象
  - 哈希值创建和比较
  - 支持不同哈希算法（MD5、SHA-1、SHA-256）
  - 大小写敏感性测试

### 2. 领域事件测试 (DomainEvents)
- **FileUploadedEventTests** - 文件上传事件测试
  - INotification 接口实现
  - 事件属性验证
  - 时间戳跟踪
  
- **FileDeletedEventTests** - 文件删除事件测试
  - 删除事件创建
  - 配额释放数据跟踪
  
- **FileSharedEventTests** - 文件分享事件测试
  - 分享事件属性验证
  - 关联ID跟踪
  
- **QuotaChangedEventTests** - 配额变更事件测试
  - 配额计算逻辑
  - 剩余空间计算
  - 使用率百分比计算
  - 边界情况处理
  
- **StorageQuotaExeededEventTests** - 配额超限事件测试
  - 超限场景验证
  - 尝试上传大小跟踪

### 3. 实体测试 (Entities)
- **FileItemTests** - 文件项实体测试
  - 聚合根特性验证
  - 软删除功能
  - 时间戳跟踪
  - 领域事件支持
  
- **UserTests** - 用户实体测试
  - Identity 集成验证
  - 用户属性测试
  - 认证功能测试
  
- **ShareLinkTests** - 分享链接实体测试
  - 聚合根特性
  - 生命周期管理
  - 事件处理

### 4. 公共组件测试 (Common)
- **BaseEntityTests** - 基础实体测试
  - ID 生成
  - 领域事件管理
  - 事件去重
  
- **AggregateRootEntityTests** - 聚合根测试
  - 软删除功能
  - 时间戳跟踪
  - 修改通知

## 🛠️ 技术栈

- **xUnit 2.6.2** - 测试框架
- **FluentAssertions 6.12.0** - 流畅的断言库
- **Moq 4.20.70** - Mock 框架
- **Microsoft.NET.Test.Sdk 17.8.0** - .NET 测试 SDK
- **coverlet.collector 6.0.0** - 代码覆盖率收集

## 🚀 运行测试

### 命令行运行所有测试
```bash
dotnet test CloudDrive.Domain.Tests/CloudDrive.Domain.Tests.csproj
```

### 运行特定测试类
```bash
dotnet test --filter "FullyQualifiedName~FileSizeTests"
```

### 运行带代码覆盖率的测试
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### 在 Visual Studio 中运行
1. 打开测试资源管理器 (Test Explorer)
2. 选择要运行的测试
3. 点击运行按钮

## 📊 测试统计

| 类别 | 测试数量 | 状态 |
|------|----------|------|
| 值对象 | 15+ | ✅ |
| 领域事件 | 30+ | ✅ |
| 实体 | 25+ | ✅ |
| 公共组件 | 20+ | ✅ |
| **总计** | **90+** | **✅** |

## 📝 测试命名约定

测试方法遵循以下命名模式：
```
[MethodName]_Should_[ExpectedBehavior]_When_[StateUnderTest]
```

示例：
- `FileSize_Should_Create_With_Valid_ByteSize`
- `QuotaChangedEvent_Should_Calculate_Usage_Percentage_Correctly`
- `SoftDelete_Should_Mark_Entity_As_Deleted`

## 🎯 测试原则

1. **AAA 模式** - Arrange（准备）、Act（执行）、Assert（断言）
2. **单一职责** - 每个测试只验证一个行为
3. **独立性** - 测试之间互不依赖
4. **可重复性** - 多次运行结果一致
5. **清晰的命名** - 测试名称描述测试内容
6. **充分的覆盖** - 覆盖正常流程和边界情况

## 🔍 持续改进

### 待添加的测试
- [ ] 实体业务方法测试（当实体添加业务方法后）
- [ ] 领域服务测试（文件去重、配额计算等）
- [ ] 集成测试（与数据库交互）
- [ ] 性能测试（大批量数据处理）

### 代码覆盖率目标
- 行覆盖率：> 80%
- 分支覆盖率：> 70%
- 方法覆盖率：> 90%

## 📚 参考资源

- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [Moq Documentation](https://github.com/moq/moq4)
- [Unit Testing Best Practices](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)

---

*最后更新时间：2025年*  
*维护者：CloudDrive 开发团队*
