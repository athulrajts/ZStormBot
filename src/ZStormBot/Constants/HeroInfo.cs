using System.Text.Json.Serialization;

namespace ZStormBot.Constants;

public class HeroInfo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("localized_name")]
    public string LocalizedName { get; set; }

    [JsonPropertyName("primary_attr")]
    public string PrimaryAttribute { get; set; }

    [JsonPropertyName("attack_type")]
    public string AttackType { get; set; }

    [JsonPropertyName("roles")]
    public string[] Roles { get; set; }

    [JsonPropertyName("img")]
    public string ThumbnailImage { get; set; }

    [JsonPropertyName("icon")]
    public string Icon { get; set; }

    [JsonPropertyName("base_health")]
    public float BaseHealth { get; set; }

    [JsonPropertyName("base_health_regen")]
    public float BaseHealthRegen { get; set; }

    [JsonPropertyName("base_mana")]
    public float BaseMana { get; set; }

    [JsonPropertyName("base_mana_regen")]
    public float BaseManaRegen { get; set; }

    [JsonPropertyName("base_armor")]
    public float BaseArmour { get; set; }

    [JsonPropertyName("base_mr")]
    public float BaseMagicResistance { get; set; }

    [JsonPropertyName("base_attack_min")]
    public float BaseAttackMin { get; set; }

    [JsonPropertyName("base_attack_max")]
    public float BaseAttackMax { get; set; }

    [JsonPropertyName("base_str")]
    public float BaseStrength { get; set; }

    [JsonPropertyName("base_agi")]
    public float BaseAgility { get; set; }

    [JsonPropertyName("base_int")]
    public float BaseIntelligence { get; set; }

    [JsonPropertyName("str_gain")]
    public float StrengthGain { get; set; }

    [JsonPropertyName("agi_gain")]
    public float AgilityGain { get; set; }

    [JsonPropertyName("int_gain")]
    public float IntelligenceGain { get; set; }

    [JsonPropertyName("attack_range")]
    public float AttackRange { get; set; }

    [JsonPropertyName("projectile_speed")]
    public float ProjectileSpeed { get; set; }

    [JsonPropertyName("attack_rate")]
    public float AttackRate { get; set; }

    [JsonPropertyName("move_speed")]
    public float MovementSpeed { get; set; }

    [JsonPropertyName("turn_rate")]
    public float? TurnRate { get; set; }

    [JsonPropertyName("cm_enabled")]
    public bool CaptainsModeEnabled { get; set; }

    [JsonPropertyName("legs")]
    public int Legs { get; set; }

    public override string ToString() => LocalizedName;
}
