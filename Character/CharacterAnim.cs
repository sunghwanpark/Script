using UnityEngine;
using System;

namespace SHProject.Ingame
{
    public enum CharacterAnimParam
    {
        [StringValue("idle")] Idle,
        [StringValue("run")] Run
    }

    public partial class CharacterBase : CommonLocatePhoton
    {
        protected Animator anim;

        private CharacterAnimParam anim_state = CharacterAnimParam.Idle;
        protected CharacterAnimParam Anim_State
        {
            get { return anim_state; }
            set
            {
                anim?.SetTrigger(value.GetStringValue());
                anim_state = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            anim = GetComponent<Animator>();
        }
    }
}