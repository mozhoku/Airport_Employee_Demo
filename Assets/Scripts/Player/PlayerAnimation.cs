using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimation : MonoBehaviour
    {
        private Animator _animator;
        private readonly int _isWalkingHash = Animator.StringToHash("IsWalking");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void UpdateAnimation(bool isWalking)
        {
            _animator.SetBool(_isWalkingHash, isWalking);
        }
    }
}