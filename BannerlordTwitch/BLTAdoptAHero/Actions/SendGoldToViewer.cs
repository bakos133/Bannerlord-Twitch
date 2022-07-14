using System;
using BannerlordTwitch;
using BannerlordTwitch.Localization;
using BannerlordTwitch.Rewards;
using BannerlordTwitch.Util;
using BLTAdoptAHero.Annotations;
using TaleWorlds.CampaignSystem;

namespace BLTAdoptAHero.Actions
{
    [LocDisplayName("{=0RJuM4U3}Send Gold"),
     LocDescription("{=noQfp9fH}Allows viewers to give some of their gold to another viewer."),
     UsedImplicitly]
    public class SendGold : HeroCommandHandlerBase
    {
        protected override void ExecuteInternal(Hero adoptedHero, ReplyContext context, object config,
            Action<string> onSuccess, Action<string> onFailure)
        {
            var heroGold = BLTAdoptAHeroCampaignBehavior.Current.GetHeroGold(adoptedHero);

            if (adoptedHero == null)
            {
                ActionManager.SendReply(context, AdoptAHero.NoHeroMessage);
                return;
            }

            if (string.IsNullOrWhiteSpace(context.Args))
            {
                ActionManager.SendReply(context,
                    context.ArgsErrorMessage("{=HAcWwjb8}(recipient) (gold Amount)".Translate()));
                return;
            }

            var argParts = context.Args.Trim().Split(' ');
            if (argParts.Length == 1)
            {
                ActionManager.SendReply(context,
                    context.ArgsErrorMessage("{=HAcWwjb8}(recipient) (gold Amount)".Translate()));
                return;
            }

            var targetHero = BLTAdoptAHeroCampaignBehavior.Current.GetAdoptedHero(argParts[0]);
            if (targetHero == null)
            {
                ActionManager.SendReply(context,
                    "{=vDsYxMKq}Couldn't find recipient '{Name}'".Translate(("Name", argParts[0])));
                return;
            }
            
            var goldAmount = int.Parse(argParts[1]);
            if (heroGold >= goldAmount)
            {
                BLTAdoptAHeroCampaignBehavior.Current.ChangeHeroGold(adoptedHero, -goldAmount, isSpending: true);
                BLTAdoptAHeroCampaignBehavior.Current.ChangeHeroGold(targetHero, goldAmount);
            }
            else
            {
                ActionManager.SendReply(context,
                    "{=yJJDmt1q}You do not have {Gold} gold. Current gold amount is {CurrentGold}"
                        .Translate(("Gold", goldAmount),
                            ("CurrentGold", heroGold)));
            }

            ActionManager.SendNonReply(context,
                "{=LYO2z7eu}'{GoldAmount}' was transferred from @{FromHero} to @{ToHero}"
                    .Translate(
                        ("GoldAmount", goldAmount),
                        ("FromHero", adoptedHero.FirstName),
                        ("ToHero", targetHero.FirstName)));
        }

        public Type HandlerConfigType => null;
    }
}