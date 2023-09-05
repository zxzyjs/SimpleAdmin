﻿// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/zxzyjs/SimpleAdmin
// 4.基于本软件的作品。，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为不要删除和修改作者声明。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.Plugin.Mqtt;

/// <summary>
/// mqtt登录参数输出
/// </summary>
public class MqttParameterOutput
{
    /// <summary>
    /// 地址
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 密码
    /// </summary>

    public string Password { get; set; }

    /// <summary>
    /// 客户端ID
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// 主题列表
    /// </summary>
    public List<string> Topics { get; set; }
}

/// <summary>
/// mqtt认证输出
/// </summary>
public class MqttAuthOutput
{
    /// <summary>
    /// 结果 "allow" | "deny" | "ignore", // Default `"ignore"`
    /// </summary>
    public string Result { get; set; } = "deny";

    /// <summary>
    /// 是否超级管理员
    /// </summary>
    public bool Is_superuser { get; set; } = false;
}