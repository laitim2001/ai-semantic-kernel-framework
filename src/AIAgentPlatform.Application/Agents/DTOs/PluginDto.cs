namespace AIAgentPlatform.Application.Agents.DTOs;

/// <summary>
/// Plugin 資訊
/// </summary>
public class PluginDto
{
    /// <summary>
    /// Plugin ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Plugin 名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Plugin 描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Plugin 類型 (native, semantic, openapi)
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Plugin 配置 (JSON)
    /// </summary>
    public string Configuration { get; set; } = string.Empty;

    /// <summary>
    /// 是否已啟用
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// 版本
    /// </summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 最後更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
