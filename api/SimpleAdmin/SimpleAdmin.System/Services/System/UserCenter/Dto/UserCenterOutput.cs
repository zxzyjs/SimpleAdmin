﻿// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/zxzyjs/SimpleAdmin
// 4.基于本软件的作品。，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为不要删除和修改作者声明。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.System;

/// <summary>
/// 我的机构数样式
/// </summary>
public class LoginOrgTreeOutput
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// 父ID
    /// </summary>
    public long Pid { get; set; }

    /// <summary>
    /// 子节点
    /// </summary>
    public List<LoginOrgTreeOutput> Children { get; set; }

    /// <summary>
    /// 样式
    /// </summary>
    public MyStyle Style { get; set; }

    /// <summary>
    /// 我的机构样式
    /// </summary>
    public class MyStyle
    {
        /// <summary>
        /// 背景色
        /// </summary>
        public string Background { get; set; } = "var(--primary-color)";

        /// <summary>
        ///  字体颜色
        /// </summary>
        public string Color { get; set; } = "#FFF";
    }
}