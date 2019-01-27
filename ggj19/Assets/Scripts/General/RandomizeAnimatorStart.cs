using UnityEngine;

namespace BrutalHack.ggj19
{
    public class RandomizeAnimatorStart : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Animator animator = GetComponent<Animator>();
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            animator.Play(stateInfo.fullPathHash, 0, Random.Range(0f,1f));
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
