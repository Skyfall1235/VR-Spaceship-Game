public interface IHealthEvents : IModuleDamage
{
    /// <summary>
    /// An event that is raised whenever the module recieves healing, along with information of how much it is healed by and at what rate.
    /// </summary>
    public OnHealEvent onHealEvent { get; set; }

    /// <summary>
    /// An event that is raised whenever the module recieves damage, along with the information on how much damage it took.
    /// </summary>
    public OnDamageEvent onDamageEvent { get; set; }

    /// <summary>
    /// An event that is raised whenever the module dies due to a lack of health
    /// </summary>
    public OnDeathEvent onDeathEvent { get; set; }
}
