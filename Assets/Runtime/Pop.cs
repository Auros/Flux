using UnityEngine;

namespace Flux
{
    [ExecuteAlways]
    public class Pop : MonoBehaviour
    {
        [SerializeField]
        private Transform _track = null!;

        [SerializeField]
        private Vector2 _offset;
        
        private void OnDrawGizmosSelected()
        {
            if (!_track)
                return;

            var tPos = transform.position;
            var pos = _track.position;
            
            var dist = Vector3.Distance(tPos, pos);
            
            var dir = (tPos - pos).normalized;
            var redEnd = pos + dir * dist;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pos, tPos);

            Gizmos.color = Color.green;
            var rot = Vector3.RotateTowards(dir, Vector3.right, 90f * Mathf.Deg2Rad, 0); // Quaternion.AngleAxis(90, Vector3.up) * dir;
            var greenEnd = pos + rot * dist;
            
            Gizmos.DrawLine(pos, greenEnd);
            
            //var rot2 = Vector3.RotateTowards(rot, Vector3.right, 90f * Mathf.Deg2Rad, 0); // Quaternion.AngleAxis(90, Vector3.up) * dir;
            //Gizmos.DrawLine(pos, pos + rot2 * 10);

            Gizmos.color = new Color(1, 1, 1, 0.2f);
            Gizmos.DrawSphere(tPos, Vector3.Distance(tPos, pos));

            Gizmos.color = Color.yellow;
            
            Gizmos.DrawLine(redEnd, greenEnd);

            var top = Vector3.Cross(rot, dir);
            Gizmos.color = Color.cyan;
            var cyanEnd = pos + top * dist;
            Gizmos.DrawLine(pos, cyanEnd);
            
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(new Vector3(_offset.x, _offset.y, 0) + pos, 0.05f);
        }
    }
}