﻿// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/zxzyjs/SimpleAdmin
// 4.基于本软件的作品。，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为不要删除和修改作者声明。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.System;

/// <summary>
/// <inheritdoc cref="IOperateLogService"/>
/// </summary>
public class OperateLogService : DbRepository<SysLogOperate>, IOperateLogService
{
    /// <summary>
    /// 操作日志中文名称
    /// </summary>
    private readonly string _nameOperate = "操作日志";

    /// <summary>
    /// 异常日志中文名称
    /// </summary>
    private readonly string _nameExecption = "异常日志";

    /// <inheritdoc />
    public async Task<SqlSugarPagedList<SysLogOperate>> Page(OperateLogPageInput input)
    {
        var query = Context.Queryable<SysLogOperate>()
            .WhereIF(!string.IsNullOrEmpty(input.Account),
                it => it.OpAccount == input.Account)//根据账号查询
            .WhereIF(!string.IsNullOrEmpty(input.Category),
                it => it.Category == input.Category)//根据分类查询
            .WhereIF(!string.IsNullOrEmpty(input.SearchKey),
                it => it.Name.Contains(input.SearchKey)
                    || it.OpIp.Contains(input.SearchKey))//根据关键字查询
            .IgnoreColumns(it => new { it.ParamJson, it.ResultJson })
            .OrderByIF(!string.IsNullOrEmpty(input.SortField),
                $"{input.SortField} {input.SortOrder}")//排序
            .OrderBy(it => it.CreateTime, OrderByType.Desc);
        var pageInfo = await query.ToPagedListAsync(input.PageNum, input.PageSize);//分页
        return pageInfo;
    }

    /// <inheritdoc />
    public async Task<List<OperateLogDayStatisticsOutput>> StatisticsByDay(int day)
    {
        //取最近七天
        var dayArray = Enumerable.Range(0, day).Select(it => DateTime.Now.Date.AddDays(it * -1))
            .ToList();
        //生成时间表
        var queryableLeft = Context.Reportable(dayArray).ToQueryable<DateTime>();
        //ReportableDateType.MonthsInLast1yea 表式近一年月份 并且queryable之后还能在where过滤
        var queryableRight = Context.Queryable<SysLogOperate>();//声名表
        //报表查询
        var list = await Context.Queryable(queryableLeft, queryableRight, JoinType.Left,
                (x1, x2)
                    => x2.CreateTime.Value.ToString("yyyy-MM-dd")
                    == x1.ColumnName.ToString("yyyy-MM-dd"))
            .GroupBy((x1, x2) => x1.ColumnName)//根据时间分组
            .OrderBy((x1, x2) => x1.ColumnName)//根据时间升序排序
            .Select((x1, x2) => new
                {
                    OperateCount =
                        SqlFunc.AggregateSum(SqlFunc.IIF(x2.Category == CateGoryConst.LOG_OPERATE,
                            1, 0)),//null的数据要为0所以不能用count
                    ExceptionCount =
                        SqlFunc.AggregateSum(SqlFunc.IIF(x2.Category == CateGoryConst.LOG_EXCEPTION,
                            1, 0)),//null的数据要为0所以不能用count
                    Date = x1.ColumnName.ToString("yyyy-MM-dd")
                }
                ).ToListAsync();
        //定义返回结果
        List<OperateLogDayStatisticsOutput> result = new List<OperateLogDayStatisticsOutput>();
        //遍历结果
        list.ForEach(it =>
        {
            result.Add(new OperateLogDayStatisticsOutput
                { Date = it.Date, Name = _nameOperate, Count = it.OperateCount });//添加访问日志
            result.Add(new OperateLogDayStatisticsOutput
                { Date = it.Date, Name = _nameExecption, Count = it.ExceptionCount });//添加异常日志
        });
        return result;
    }

    /// <inheritdoc />
    public async Task<List<OperateLogTotalCountOutpu>> TotalCount()
    {
        var data = await Context.Queryable<SysLogOperate>()
            .GroupBy(it => it.Category)//根据分类分组
            .Select(it => new
            {
                Category = it.Category,//分类
                Count = SqlFunc.AggregateCount(it.Category)//数量
            }).ToListAsync();
        //定义结果数组
        List<OperateLogTotalCountOutpu> operageLogTotalCounts = new List<OperateLogTotalCountOutpu>
        {
            //添加操作日志数据
            new OperateLogTotalCountOutpu
            {
                Type = _nameOperate,
                Value = data.Where(it => it.Category == CateGoryConst.LOG_OPERATE)
                    .Select(it => it.Count).FirstOrDefault()
            },
            //添加异常日志数据
            new OperateLogTotalCountOutpu
            {
                Type = _nameExecption,
                Value = data.Where(it => it.Category == CateGoryConst.LOG_EXCEPTION)
                    .Select(it => it.Count).FirstOrDefault()
            }
        };
        return operageLogTotalCounts;
    }

    /// <inheritdoc />
    public async Task Delete(string category)
    {
        await DeleteAsync(it => it.Category == category);//删除对应分类日志
    }

    /// <inheritdoc />
    public async Task<SysLogOperate> Detail(BaseIdInput input)
    {
        return await GetFirstAsync(it => it.Id == input.Id);//删除对应分类日志
    }
}
