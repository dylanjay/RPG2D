using UnityEngine;

namespace Benco.BehaviorTree
{
    [CreateAssetMenu(fileName = "New Action Move Towards Player", menuName = "Actions/MoveTowardsPlayer", order = 2)]
    [ShowInNodeEditor("Move Towards Player", false)]
    public class ActionMoveTowardsPlayer : BehaviorLeaf
    {
        [SerializeField]
        private SharedTransform player;

        public override void OnStart()
        {
            entity.anim.SetTrigger(Hostile.AnimParams.Alert);
        }

        public override BehaviorState Update()
        {
            entity.transform.position = Vector2.MoveTowards(transform.position, player.value.position, entity.moveSpeed.value * Time.deltaTime);

            return BehaviorState.Running;
        }
    }
}
