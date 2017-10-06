using HouraiTeahouse.SmashBrew;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

    public enum TouhouGame {
        HighlyResponsiveToPrayers = 0,
        StoryOfEasternWonderland,
        PhantasmagoriaOfDimDream,
        LotusLandStory,
        MysticSquare,
        EmbodimentOfScarletDevil,
        PerfectCherryBlossom,
        ImmaterialAndMissingPower,
        ImperishableNight,
        PhantasmagoriaOfFlowerView,
        ShootTheBullet,
        MountainOfFaith,
        ScarletWeatherRhapsody,
        SubterraneanAnimism,
        UndefinedFantasticObject,
        Hisoutensoku,
        DoubleSpoiler,
        FairyWars,
        TenDesires,
        HopelessMasquerade,
        ImpossibleSpellCard,
        UrbanLegendInLimbo,
        LegacyOfLunaticKingdom,
        Other
    }

    public enum TouhouStage {
        PlayableCharacter = 0,
        Stage1,
        Stage2,
        Stage3,
        Stage4,
        Stage5,
        Stage6,
        Extra,
        Other
    }

    /// <summary>
    /// A ScriptableObject extenstion of CharacterData that holds information regarding a Touhou
    /// character's source information.
    /// </summary>
    [Extension(typeof(CharacterData))]
    public class TouhouCharacterData : ScriptableObject {

        [SerializeField]
        [Tooltip("The original game or media the character is from ")]
        TouhouGame _sourceGame = TouhouGame.Other;

        [SerializeField]
        [Tooltip("The source stage the character is from ")]
        TouhouStage _sourceStage = TouhouStage.Other;

        public TouhouGame SourceGame {
            get { return _sourceGame; }
        }
        public TouhouStage SourceStage {
            get { return _sourceStage; }
        }

    }

}
