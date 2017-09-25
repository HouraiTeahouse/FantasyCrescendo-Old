using NUnit.Framework;

namespace HouraiTeahouse.SmashBrew {

    /// <summary> 
    /// Tests for CharacterData instances.
    /// </summary>
    /// <remarks>
    /// Note: These test function as validation for on the data available at build time.
    /// If the data is invalid, these tests will fail.
    /// </remarks>
    internal class SceneDataTest : AbstractDataTest<SceneData> {

        [Test, TestCaseSource("TestData")]
        public void has_valid_preview_image(SceneData scene) => 
            Assert.NotNull(scene.PreviewImage.Load());

        [Test, TestCaseSource("TestData")]
        public void has_valid_icon(SceneData scene) =>  
            Assert.NotNull(scene.PreviewImage.Load());

    }

}