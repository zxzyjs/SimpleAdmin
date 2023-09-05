﻿// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/zxzyjs/SimpleAdmin
// 4.基于本软件的作品。，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为不要删除和修改作者声明。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.System;

/// <inheritdoc cref="IRelationService"/>
public class Relationservice : DbRepository<SysRelation>, IRelationService
{
    private readonly ILogger<Relationservice> _logger;
    private readonly ISimpleCacheService _simpleCacheService;
    private readonly IResourceService _resourceService;

    public Relationservice(ILogger<Relationservice> logger, ISimpleCacheService simpleCacheService, IResourceService resourceService)
    {
        _logger = logger;
        _simpleCacheService = simpleCacheService;
        _resourceService = resourceService;
    }

    /// <inheritdoc/>
    public async Task<List<SysRelation>> GetRelationByCategory(string category)
    {
        var key = SystemConst.Cache_SysRelation + category;
        //先从Redis拿
        var sysRelations = _simpleCacheService.Get<List<SysRelation>>(key);
        if (sysRelations == null)
        {
            //redis没有就去数据库拿
            sysRelations = await base.GetListAsync(it => it.Category == category);
            if (sysRelations.Count > 0)
            {
                //插入Redis
                _simpleCacheService.Set(key, sysRelations);
            }
        }
        return sysRelations;
    }

    /// <inheritdoc/>
    public async Task<List<SysRelation>> GetRelationListByObjectIdAndCategory(long objectId, string category)
    {
        var sysRelations = await GetRelationByCategory(category);
        var result = sysRelations.Where(it => it.ObjectId == objectId).ToList();//获取关系集合
        return result;
    }

    /// <inheritdoc/>
    public async Task<SysRelation> GetWorkbench(long userId)
    {
        var sysRelations = await GetRelationByCategory(CateGoryConst.Relation_SYS_USER_WORKBENCH_DATA);
        var result = sysRelations.Where(it => it.ObjectId == userId).FirstOrDefault();//获取个人工作台
        return result;
    }

    /// <inheritdoc/>
    public async Task<List<SysRelation>> GetRelationListByObjectIdListAndCategory(List<long> objectIds, string category)
    {
        var sysRelations = await GetRelationByCategory(category);
        var result = sysRelations.Where(it => objectIds.Contains(it.ObjectId)).ToList();//获取关系集合
        return result;
    }

    /// <inheritdoc/>
    public async Task<List<SysRelation>> GetRelationListByTargetIdAndCategory(string targetId, string category)
    {
        var sysRelations = await GetRelationByCategory(category);
        var result = sysRelations.Where(it => it.TargetId == targetId).ToList();//获取关系集合
        return result;
    }

    /// <inheritdoc/>
    public async Task<List<SysRelation>> GetRelationListByTargetIdListAndCategory(List<string> targetIds, string category)
    {
        var sysRelations = await GetRelationByCategory(category);
        var result = sysRelations.Where(it => targetIds.Contains(it.TargetId)).ToList();//获取关系集合
        return result;
    }

    /// <inheritdoc/>
    public async Task RefreshCache(string category)
    {
        var key = SystemConst.Cache_SysRelation + category;//key
        _simpleCacheService.Remove(key);//删除redis
        await GetRelationByCategory(category);//更新缓存
    }

    /// <inheritdoc/>
    public async Task SaveRelationBatch(string category, long objectId, List<string> targetIds,
        List<string> extJsons, bool clear)
    {
        var sysRelations = new List<SysRelation>();//要添加的列表
        for (var i = 0; i < targetIds.Count; i++)
        {
            sysRelations.Add(new SysRelation
            {
                ObjectId = objectId,
                TargetId = targetIds[i],
                Category = category,
                ExtJson = extJsons == null ? null : extJsons[i]
            });
        }
        //事务
        var result = await itenant.UseTranAsync(async () =>
        {
            if (clear)
                await DeleteAsync(it => it.ObjectId == objectId && it.Category == category);//删除老的
            await InsertRangeAsync(sysRelations);//添加新的
        });
        if (result.IsSuccess)//如果成功了
        {
            await RefreshCache(category);
        }
        else
        {
            //写日志
            _logger.LogError(result.ErrorMessage, result.ErrorException);
            throw Oops.Oh(ErrorCodeEnum.A0003);
        }
    }

    /// <inheritdoc/>
    public async Task SaveRelation(string category, long objectId, string targetId,
        string extJson, bool clear, bool refreshCache = true)
    {
        var sysRelation = new SysRelation
        {
            ObjectId = objectId,
            TargetId = targetId,
            Category = category,
            ExtJson = extJson
        };
        //事务
        var result = await itenant.UseTranAsync(async () =>
        {
            if (clear)
                await DeleteAsync(it => it.ObjectId == objectId && it.Category == category);//删除老的
            await InsertAsync(sysRelation);//添加新的
        });
        if (result.IsSuccess)//如果成功了
        {
            if (refreshCache)
                await RefreshCache(category);
        }
        else
        {
            //写日志
            _logger.LogError(result.ErrorMessage, result.ErrorException);
            throw Oops.Oh(ErrorCodeEnum.A0003);
        }
    }

    /// <inheritdoc/>
    public async Task<List<long>> GetModuleByRoleId(List<long> roleIdList)
    {
        var moduleIds = new List<long>();
        var relation = await GetRelationByCategory(CateGoryConst.Relation_SYS_ROLE_HAS_MODULE);//获取关系集合
        if (relation != null && relation.Count > 0)
        {
            moduleIds = relation.Where(it => roleIdList.Contains(it.ObjectId)).Select(it => it.TargetId.ToLong()).ToList();
        }
        return moduleIds;
    }
}