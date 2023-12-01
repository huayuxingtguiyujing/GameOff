using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace GameOff2023.Character {
    public class InputHandler : MonoBehaviour
    {

        [Header("来自外界的输入")]
        public Vector2 moveInput = default;
        public bool attackInput = default;
        public bool obtainInput = default;
        public bool jumpInput = default;
        public bool interactInput = default;
        public bool defenseInput = default;

        public bool num1Input = default;
        public bool num2Input = default;
        public bool num3Input = default;
        public bool num4Input = default;
        public bool num5Input = default;
        public bool switchLeftInput = default;
        public bool switchRightInput = default;

        #region Input System 事件
        void OnMove(InputValue value) {
            moveInput = value.Get<Vector2>();
            //Debug.Log("OnMove : " + value.Get<Vector2>());
        }

        void OnAttack(InputValue value) {
            attackInput = value.Get<float>() > 0.1f ? true : false;
        }

        void OnObtain(InputValue value) {
            obtainInput = value.Get<float>() > 0.1f ? true : false;
        }

        void OnJump(InputValue value) {
            jumpInput = value.Get<float>() > 0.1f ? true : false;
        }

        void OnInteract(InputValue value) {
            interactInput = value.Get<float>() > 0.1f ? true : false;
        }

        void OnDefense(InputValue value) {
            defenseInput = value.Get<float>() > 0.1f ? true : false;
            //Debug.Log("OnDefense : " + value.Get<bool>());
        } 
        
        
        void OnNum1(InputValue value) {
            num1Input = value.Get<float>() > 0.1f ? true : false;
        }
        void OnNum2(InputValue value) {
            num2Input = value.Get<float>() > 0.1f ? true : false;
        }
        void OnNum3(InputValue value) {
            num3Input = value.Get<float>() > 0.1f ? true : false;
        }
        void OnNum4(InputValue value) {
            num4Input = value.Get<float>() > 0.1f ? true : false;
        }
        void OnNum5(InputValue value) {
            num5Input = value.Get<float>() > 0.1f ? true : false;
        }

        void OnSwitchLeft(InputValue value) {
            switchLeftInput = value.Get<float>() > 0.1f ? true : false;
        }
        void OnSwitchRight(InputValue value) {
            switchRightInput = value.Get<float>() > 0.1f ? true : false;
        }
        #endregion

        public void ResetAllTrigger() {
            attackInput = false;
            obtainInput = false;

            jumpInput = false;
            interactInput = false;
            defenseInput = false;

            num1Input = false; 
            num2Input = false;
            num3Input = false;
            num4Input = false;
            num5Input = false;

            switchLeftInput = false;
            switchRightInput = false;
        }
    }

}
