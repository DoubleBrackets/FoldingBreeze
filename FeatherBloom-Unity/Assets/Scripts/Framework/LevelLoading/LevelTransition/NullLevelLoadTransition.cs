using Cysharp.Threading.Tasks;

namespace Framework.LevelLoading.LevelTransition
{
    public class NullLevelLoadTransition : ILevelLoadTransition
    {
        public UniTask TransitionOutFromCurrentScene()
        {
            return UniTask.CompletedTask;
        }

        public UniTask TransitionInToNewScene()
        {
            return UniTask.CompletedTask;
        }
    }
}