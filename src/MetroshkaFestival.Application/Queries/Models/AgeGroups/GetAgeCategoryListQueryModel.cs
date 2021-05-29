﻿using System;
using System.ComponentModel.DataAnnotations;
using Interfaces.Application;
using MetroshkaFestival.Application.WebModels.AgeCategories;
using MetroshkaFestival.Core.Models.Common;
using MetroshkaFestival.Data.Entities;

namespace MetroshkaFestival.Application.Queries.Models.AgeGroups
{
    public class GetAgeCategoryListQueryModel : ReturnQueryModel, IQuery<GetAgeCategoryListQueryResult>
    {
        public AgeCategorySortQueryModel Sort { get; init; }
    }

    public class GetAgeCategoryListQueryResult : QueryResult<AgeCategoryListModel>
    {
        public static GetAgeCategoryListQueryResult BuildResult(AgeCategoryListModel result = null, string error = null)
        {
            return error != null ? Failed<GetAgeCategoryListQueryResult>(error) : Ok<GetAgeCategoryListQueryResult>(result);
        }
    }

    public record AddAgeCategoryModel(string ReturnUrl,
        [Required]
        AgeGroup? AgeGroup = null,
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Год не может быть отрицательным")]
        int? MinBirthYear = null,
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Год не может быть отрицательным")]
        int? MaxBirthYear = null);

    public class AgeCategoryListModel
    {
        public GetAgeCategoryListQueryModel Query { get; set; }
        public AgeCategory[] AgeCategories { get; set; }
    }
}