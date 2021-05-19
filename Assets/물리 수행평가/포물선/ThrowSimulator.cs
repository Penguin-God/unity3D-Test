using UnityEngine;
using System.Collections;

public class ThrowSimulator : MonoBehaviour
{
    // 프로그램밍 언어에서 = 은 같다는 뜻이 아닌 변수에 = 뒤에 값을 대입하겠다는 뜻을 의미 같다는 뜻을 의미하는 기호는 ==
    
    // ex)
    // x = 2; 
    // y = 3;
    // x + y == 5 가 됨
    public Transform Target; // 도착점
    public float gravity = 9.8f; // 중력 가속도

    void Start() // 게임이 시작될 때 코드 실행
    {
        StartCoroutine(SimulateProjectile());
    }

    IEnumerator SimulateProjectile()
    { 


        // 1. 투사체와 도착점 사이의 거리 계산
        // this.transform.position 운동하는 물체 즉 자신의 백터값, Target.position은 도착점의 백터값
        // Vector3.Distance() : 괄호 안에 들어가는 두 백터값의 거리를 구해주는 코드
        // 이 코드는 운동이 시작되기 전에 실행되므로 시작점과 도착점 사이의 거리를 구함
        // target_Distance = 시작점과 도착점 사이의 거리
        float target_Distance = Vector3.Distance(this.transform.position, Target.position);

        // 2. 지정된 각도를 공식에 대입해 물체를 던지는 데 필요한 속도를 계산
        // R = ( V(초기 속도) * V(초기 속도) * sin2θ ) / g  공식을 이용함
        // R(target_Distance)은 위에서 구하여 알고 있으며 θ는 45°일 때 수평 도달 거리가 최대가 되므로 θ에 45를 대입하여 V(초기 속도)의 제곱을 구함
        // 공식을 변형하여 V * V = R * (g / sin2θ) 를 사용하여 속도의 제곱을 구함
        float projectile_Velocity = target_Distance * (gravity / Mathf.Sin(2 * 45 * Mathf.Deg2Rad));
        // Mathf.Sin() : ()안의 sin값을 반환해주는 코드
        // Mathf.Deg2Rad : 각도(degree)에 곱할 시 각도를 라디안으로 바꿔주는 상수
        // (Mathf.Sin() 코드 안에는 라디안 값을 넣어야 하기 때문에 각도를 라디안으로 바꿈)
        // projectile_Velocity = 속도의 제곱값


        // 3. 속도의 X Y 요소 계산
        // Vx = V(초기속도)cosθ, Vy = V(초기속도)sinθ 공식을 이용
        // Mathf.Sqrt() : 괄호 안의 값의 제곱근을 구함 
        // 위에서 구한 projectile_Velocity은 속도의 제곱값이였기 때문에 제곱급 값을 구해서 공식 이용
        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(45 * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(45 * Mathf.Deg2Rad);

        // 4. 포물선 운동 시간을 계산
        // R(이동거리) = Vx * 2t 공식을 이용 (t는 최고점까지 올라가는데 걸리는 시간 즉, 2t는 포물선 운동시간)
        // 2t(운동 시간) = R / Vx()
        // flightDuration = 포물선 운동시간
        float flightDuration = target_Distance / Vx;







        // 5. 실제 운동
        // ()안의 조건이 만족 되면(조건이 참이라면) {} 안의 코드가 1프레임마다 실행됨 
        // elapse_time < flightDuration : 위에서 구한 운동시간이 운동을 시작한 후 시간보다 크면 조건이 참임
        float elapse_time = 0;
        while (elapse_time < flightDuration)
        {
            elapse_time = elapse_time + Time.deltaTime;
            // Time.deltaTime : 지난 프레임부터 이번 프레임까지의 시간 대략 1/60초
            // 프로그램밍 언어에서 = 은 같다는 뜻이 아닌 변수에 = 뒤에 값을 대입하겠다는 뜻이므로 자기 자신에 프레임 간의 시간차이 를 더하는 코드
            // 매 프레임마다 elapse_time에 프레임간의 시간차이를 더하는 코드이므로 elapse_time 변수는 걸린 시간(경과 시간)이 됨


            // x = (V(초기속도) * cosθ) * t, y = (V(초기속도) * sinθ) - g * t * t / 2 공식을 사용
            // 여기서 t = elapse_time(경과 시간), (V(초기속도) * cosθ) = Vx, (V(초기속도) * sinθ) = Vy
            // this.transform.position = new Vector3() 코드는 자신의 현재 위치를 ()안의 백터값으로 바꾸는 코드
            this.transform.position = new Vector3(-Vx * elapse_time, (Vy * elapse_time) - (elapse_time * elapse_time * gravity) / 2, 0);
            // x좌표의 -는 방향을 위해서 곱함

            yield return null; // 1프레임 멈추기
        }











    }
}