using NUnit.Framework;
using Unity.Multiplayer.Tools.NetworkProfiler.Editor;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Editor
{
    class DetailsViewFoldoutStateTests
    {
        const string k_FoldoutA = nameof(k_FoldoutA);
        const string k_FoldoutB = nameof(k_FoldoutB);

        [Test]
        public void GivenAllFoldoutsExpanded_WhenIsFoldedOutCalled_ReturnsTrue()
        {
            var detailsViewFoldoutState = new DetailsViewFoldoutState();
            detailsViewFoldoutState.SetFoldoutExpandAll();

            var isFoldedOut = detailsViewFoldoutState.IsFoldedOut(k_FoldoutA);
            
            Assert.IsTrue(isFoldedOut);
        }

        [Test]
        public void GivenAllFoldoutsExpandedExceptOne_WhenIsFoldoutCalledForThatOne_ReturnsFalse()
        {
            var detailsViewFoldoutState = new DetailsViewFoldoutState();
            detailsViewFoldoutState.SetFoldoutExpandAll();
            detailsViewFoldoutState.SetFoldout(k_FoldoutA, false);

            var isFoldedOut = detailsViewFoldoutState.IsFoldedOut(k_FoldoutA);
            
            Assert.IsFalse(isFoldedOut);
        }
        
        [Test]
        public void GivenAllFoldoutsExpandedExceptOne_WhenIsFoldoutCalledForOthers_ReturnsTrue()
        {
            var detailsViewFoldoutState = new DetailsViewFoldoutState();
            detailsViewFoldoutState.SetFoldoutExpandAll();
            detailsViewFoldoutState.SetFoldout(k_FoldoutA, false);

            var isFoldedOut = detailsViewFoldoutState.IsFoldedOut(k_FoldoutB);
            
            Assert.IsTrue(isFoldedOut);
        }

        [Test]
        public void GivenAllFoldoutsContracted_WhenIsFoldoutCalled_ReturnsFalse()
        {
            var detailsViewFoldoutState = new DetailsViewFoldoutState();
            detailsViewFoldoutState.SetFoldoutContractAll();

            var isFoldedOut = detailsViewFoldoutState.IsFoldedOut(k_FoldoutA);
            
            Assert.IsFalse(isFoldedOut);
        }

        [Test]
        public void GivenAllFoldoutsContractedExceptOne_WhenIsFoldoutCalledForThatOne_ReturnsTrue()
        {
            var detailsViewFoldoutState = new DetailsViewFoldoutState();
            detailsViewFoldoutState.SetFoldoutContractAll();
            detailsViewFoldoutState.SetFoldout(k_FoldoutA, true);

            var isFoldedOut = detailsViewFoldoutState.IsFoldedOut(k_FoldoutA);
            
            Assert.IsTrue(isFoldedOut);
        }
        
        [Test]
        public void GivenAllFoldoutsContractedExceptOne_WhenIsFoldoutCalledForOthers_ReturnsFalse()
        {
            var detailsViewFoldoutState = new DetailsViewFoldoutState();
            detailsViewFoldoutState.SetFoldoutContractAll();
            detailsViewFoldoutState.SetFoldout(k_FoldoutA, true);

            var isFoldedOut = detailsViewFoldoutState.IsFoldedOut(k_FoldoutB);
            
            Assert.IsFalse(isFoldedOut);
        }
    }
}
