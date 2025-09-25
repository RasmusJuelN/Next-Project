using Database.Attributes;
using Database.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Database.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum QuestionnaireGroupOrderingOptions
{
    [QueryMethod(nameof(IQueryableExtensions.OrderByNameAsc))]
    NameAsc,
    [QueryMethod(nameof(IQueryableExtensions.OrderByNameDesc))]
    NameDesc,
    [QueryMethod(nameof(IQueryableExtensions.OrderByCreatedAtAsc))]
    CreatedAtAsc,
    [QueryMethod(nameof(IQueryableExtensions.OrderByCreatedAtDesc))]
    CreatedAtDesc
}