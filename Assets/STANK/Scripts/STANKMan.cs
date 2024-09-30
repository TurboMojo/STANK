using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace STANK {
    public class STANKMan : MonoBehaviour
    {
        // Character controller for prototyping and demonstrating STANK.
        public float moveSpeed;
        public float horizontalRotateSpeed = 10f;
        public float verticalRotateSpeed = 7f;
        public float burstSpeed;
        public GameObject eyeballs;
    
        private Vector2 bodyRotation = new Vector2(0,0);
        private Vector2 headRotation = new Vector2(0,0);
        private Vector2 m_Look;
        private Vector2 m_Move;

        public void OnMove(InputAction.CallbackContext context)
        {
            
            m_Move = context.ReadValue<Vector2>();
            
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            m_Look = context.ReadValue<Vector2>();

        }

        public void Update()
        {
            // Update orientation first, then move. Otherwise move orientation will lag
            // behind by one frame.
            
            Look(m_Look);
            Move(m_Move);
        }

        private void Move(Vector2 direction)
        {
            if (direction.sqrMagnitude < 0.01)
                return;
            var scaledMoveSpeed = moveSpeed * Time.deltaTime;
            // For simplicity's sake, we just keep movement in a single plane here. Rotate
            // direction according to world Y rotation of player.
            var move = Quaternion.Euler(0, transform.eulerAngles.y, 0) * new Vector3(direction.x, 0, direction.y);
            transform.position += move * scaledMoveSpeed;
        }

        private void Look(Vector2 rotate)
        {
            if (rotate.sqrMagnitude < 0.01)
                return;
            var scaledHorizontalRotateSpeed = horizontalRotateSpeed * Time.deltaTime;
            bodyRotation.y += rotate.x * scaledHorizontalRotateSpeed;            
            transform.localEulerAngles = bodyRotation;

            var scaledVerticalRotateSpeed = verticalRotateSpeed * Time.deltaTime;
            
            headRotation.x = Mathf.Clamp(headRotation.x - rotate.y * scaledVerticalRotateSpeed, -89, 89);
            eyeballs.transform.localEulerAngles = headRotation;
        }
    }
}