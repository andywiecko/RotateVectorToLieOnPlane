using Unity.Mathematics;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    private bool IsValid => math.pow(math.dot(v, r), 2) <= 1 - math.pow(math.dot(n, r), 2);
    private float3 v => math.normalize(vector);
    private float3 n => transform.up;
    private float3 r => math.normalize(rotation);

    [SerializeField]
    private float3 vector = Vector3.up + Vector3.forward;

    [SerializeField]
    private float3 rotation = Vector3.up + Vector3.forward;

    private void OnValidate()
    {
        if (math.all(vector == float3.zero))
        {
            vector = Vector3.up;
        }

        if (math.all(rotation == float3.zero))
        {
            rotation = Vector3.up;
        }
    }

    private bool TryFindRotationAngle(out float theta1, out float theta2)
    {
        if (!IsValid)
        {
            theta1 = float.NaN;
            theta2 = float.NaN;
            return false;
        }

        var A = math.dot(v, n);
        var B = math.dot(math.cross(r, v), n);
        var C = math.dot(v, r) * math.dot(n, r);

        var tmp1 = (A - C);
        var tmp2 = B * B;
        var tmp3 = tmp1 * tmp1 + tmp2;
        var delta = math.sqrt(tmp2 * (tmp3 - C * C));

        theta1 = math.acos((-C * tmp1 - delta) / tmp3);
        theta2 = math.acos((-C * tmp1 + delta) / tmp3);
        return true;
    }

    private quaternion Euler(float3 direction, float theta) => new quaternion(math.float4(direction * math.sin(theta / 2f), math.cos(theta / 2f)));

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawRay(transform.position, v);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, r);

        Gizmos.color = IsValid ? Color.blue : Color.red;
        Gizmos.DrawRay(transform.position, n);

        if (TryFindRotationAngle(out var theta1, out var theta2))
        {
            var v1 = math.mul(Euler(r, theta1), v);
            var v2 = math.mul(Euler(r, theta2), v);
            var solution = math.abs(math.dot(v1, n)) < math.abs(math.dot(v2, n)) ? v1 : v2;

            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(transform.position, math.normalize(solution));
        }
    }
}
