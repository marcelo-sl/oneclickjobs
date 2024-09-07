using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace OneClickJobs.Web.Data.Contexts;

public class EnumToStringConverter<TEnum> : ValueConverter<TEnum, string>
    where TEnum : struct, Enum
{
    public EnumToStringConverter()
        : base(
            v => v.ToString(),
            v => (TEnum)Enum.Parse(typeof(TEnum), v))
    {
    }
}
