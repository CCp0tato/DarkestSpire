using DarkestSpire.Characters.Crusader.Relics;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Entities.RestSite;

#nullable enable
namespace DarkestSpire.Characters.Events;

public class ExampleRestSiteOption(Player owner) : RestSiteOption(owner)
{
  public override string OptionId => "EXAMPLEOPTION";

  public override LocString Description
  {
    get 
    {
      LocString description = base.Description;
      // description.Add("RekindleAmount", 5M);
      return description;
    }
  }

  public override Task<bool> OnSelect()
  {
    this.Owner.GetRelic<CrusaderStarterRelic>()?.Heal(10M);
    return Task.FromResult<bool>(true);
  }

  public override Task DoLocalPostSelectVfx(CancellationToken ct = default (CancellationToken))
  {
    // this.ExamplePlayVfx();
    return Task.CompletedTask;
  }

  public override Task DoRemotePostSelectVfx()
  {
    // this.ExamplePlayVfx();
    return Task.CompletedTask;
  }

  // private void ExamplePlayVfx()
  // {
  //   SfxCmd.Play("event:/sfx/characters/attack_fire");
  //   NRestSiteRoom instance = NRestSiteRoom.Instance;
  //   NRestSiteCharacter parent = instance != null ? instance.Characters.First<NRestSiteCharacter>((Func<NRestSiteCharacter, bool>) (c => c.Player == this.Owner)) : (NRestSiteCharacter) null;
  //   parent?.Shake();
  //   NRelicFlashVfx child = NRelicFlashVfx.Create((RelicModel) ModelDb.Relic<CrusaderStarterRelic>());
  //   if (child == null)
  //     return;
  //   if (parent != null)
  //     parent.AddChildSafely((Node) child);
  //   child.Position = Vector2.Zero;
  // }
}
