using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collplex.Models
{
    // BaseServiceEnums(5):
    // 用户信息 user
    // 课表 schedule
    // 成绩 score
    // 一卡通 ecard
    // 考试日程 exam
    public enum BaseServiceEnum
    {
        user,
        schedule,
        score,
        ecard,
        exam
    }
}
