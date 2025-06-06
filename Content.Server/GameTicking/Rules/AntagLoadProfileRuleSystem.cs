using Content.Server.Antag;
using Content.Server.GameTicking.Rules.Components;
using Content.Server.Humanoid;
using Content.Server.Preferences.Managers;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Preferences;
using Robust.Shared.Prototypes;

namespace Content.Server.GameTicking.Rules;

public sealed class AntagLoadProfileRuleSystem : GameRuleSystem<AntagLoadProfileRuleComponent>
{
    [Dependency] private readonly HumanoidAppearanceSystem _humanoid = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly IServerPreferencesManager _prefs = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AntagLoadProfileRuleComponent, AntagSelectEntityEvent>(OnSelectEntity);
    }

    private void OnSelectEntity(Entity<AntagLoadProfileRuleComponent> ent, ref AntagSelectEntityEvent args)
    {
        if (args.Handled)
            return;

        var profile = args.Session != null
            ? _prefs.GetPreferences(args.Session.UserId).SelectedCharacter as HumanoidCharacterProfile
            : HumanoidCharacterProfile.RandomWithSpecies();


        if (profile?.Species is not { } speciesId || !_proto.TryIndex<SpeciesPrototype>(speciesId, out var species))
        {
            species = _proto.Index<SpeciesPrototype>(SharedHumanoidAppearanceSystem.DefaultSpecies);
        }

        if (ent.Comp.SpeciesOverride != null
            && (ent.Comp.AlwaysUseSpeciesOverride || ( ent.Comp.SpeciesOverrideBlacklist?.Contains(new (species.ID)) ?? false))) // Goob edit
        {
            species = _proto.Index(ent.Comp.SpeciesOverride.Value);
        }

        if (ent.Comp.SpeciesHardOverride is not null) // Shitmed - Starlight Abductors
            species = _proto.Index(ent.Comp.SpeciesHardOverride.Value); // Shitmed - Starlight Abductors

        if (ent.Comp.SpeciesHardOverride is not null) // Shitmed - Starlight Abductors
            species = _proto.Index(ent.Comp.SpeciesHardOverride.Value); // Shitmed - Starlight Abductors

        args.Entity = Spawn(species.Prototype);
        _humanoid.LoadProfile(args.Entity.Value, profile?.WithSpecies(species.ID));
    }
}
