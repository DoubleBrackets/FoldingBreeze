using Cysharp.Threading.Tasks;

namespace Framework.LevelLoading.LevelTransition
{
    public interface ILevelLoadTransition
    {
        public UniTask TransitionOutFromCurrentScene();

        public UniTask TransitionInToNewScene();
    }
}