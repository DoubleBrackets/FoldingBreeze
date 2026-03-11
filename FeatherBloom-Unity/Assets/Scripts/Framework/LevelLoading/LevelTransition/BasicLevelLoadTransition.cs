using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Framework.LevelLoading.LevelTransition
{
    public class BasicLevelLoadTransition : MonoBehaviour, ILevelLoadTransition
    {
        [SerializeField]
        private float _transitionTime;

        [SerializeField]
        private CanvasGroup _loadingScreenCanvasGroup;

        public async UniTask TransitionOutFromCurrentScene()
        {
            _loadingScreenCanvasGroup.alpha = 0f;
            float timer = _transitionTime;

            while (timer > 0f)
            {
                timer -= Time.deltaTime;
                float alpha = 1 - Mathf.Clamp01(timer / _transitionTime);
                _loadingScreenCanvasGroup.alpha = alpha;
                await UniTask.Yield();
            }
        }

        public async UniTask TransitionInToNewScene()
        {
            _loadingScreenCanvasGroup.alpha = 1f;
            float timer = _transitionTime;

            while (timer > 0f)
            {
                timer -= Time.deltaTime;
                float alpha = Mathf.Clamp01(timer / _transitionTime);
                _loadingScreenCanvasGroup.alpha = alpha;
                await UniTask.Yield();
            }
        }
    }
}