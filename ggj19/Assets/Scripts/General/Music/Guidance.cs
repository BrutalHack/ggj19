using UnityEngine;
using UnityEngine.Analytics;

namespace BrutalHack.ggj19.General.Music
{
    public class Guidance : MonoBehaviour
    {
        public TimedNote note;
        private Animator animator;
        private static readonly int Hit1 = Animator.StringToHash("hit");
        private static readonly int Miss1 = Animator.StringToHash("miss");

        public float guidanceMissTimer = 1.31f;
        private bool hit;

        public delegate void MissDelegate(Guidance guidance);

        public event MissDelegate OnMiss;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            var position = rectTransform.position;
            rectTransform.position = new Vector3(position.x - Time.deltaTime * 8f, position.y, position.z);
            if (!hit)
            {
                if (guidanceMissTimer > 0f)
                {
                    guidanceMissTimer -= Time.deltaTime;
                }
                else
                {
                    Miss();
                }
            }
        }

        public void Hit()
        {
            animator.SetTrigger(Hit1);
            hit = true;
        }

        public void Miss()
        {
            Debug.Log("Miss called");
            animator.SetTrigger(Miss1);
            OnMiss?.Invoke(this);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}