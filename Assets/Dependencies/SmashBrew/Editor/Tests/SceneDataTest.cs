using NUnit.Framework;

namespace HouraiTeahouse.SmashBrew {

    internal class SceneDataTest : AbstractDataTest<SceneData> {

        [Test]
        public void every_scene_has_valid_preview_image() => 
            Check(s => s.PreviewImage.Load());

        [Test]
        public void every_scene_has_valid_icon() =>  
            Check(s => s.Icon.Load());

    }

}