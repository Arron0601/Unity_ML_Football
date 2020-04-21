
using UnityEngine;
using MLAgents;
using MLAgents.Sensors;

public class Robot :Agent
{
    [Header("速度"), Range(1, 50)]
    public float speed = 10;

    /// <summary>
    /// 機器人鋼體
    /// </summary>
    private Rigidbody rigRobot;
    /// <summary>
    /// 足球剛體
    /// </summary>
    private Rigidbody rigBall;

    private void Start()
    {
        rigRobot = GetComponent<Rigidbody>(); //抓到本身原件
        rigBall = GameObject.Find("足球").GetComponent<Rigidbody>();//找到足球物件剛體
    }
    /// <summary>
    /// 事件開始時:重新設定機器人跟足球位置
    /// </summary>
    public override void OnEpisodeBegin()
    {
        //重設剛體加速度與角度加速度(防止亂旋轉)
        rigRobot.velocity = Vector3.zero; //加速度歸零
        rigRobot.angularVelocity = Vector3.zero;//角度加速度歸零
        rigBall.velocity = Vector3.zero;
        rigBall.angularVelocity = Vector3.zero;


        //給予機器人跟足球新的隨機座標
        Vector3 posRobot = new Vector3(Random.Range(-2f, 2f), 0.1f,Random.Range(-2f,0f));
        transform.position = posRobot;//指定機器人座標

        Vector3 posBall = new Vector3(Random.Range(-2f, 2f), 0.1f, Random.Range(1f, 2f));
        rigBall.position = posBall;//球的座標變成算出來的隨機座標

        BALL.complete = false; //尚未成功
    }

    /// <summary>
    /// 蒐集、觀測資料
    /// </summary>
    public override void CollectObservations(VectorSensor sensor)
    {
        //加入觀測資料: 機器人、足球座標、機器人加速度x、z
        sensor.AddObservation(transform.position);//將本身座標加入觀測資料
        sensor.AddObservation(rigBall.position);
        sensor.AddObservation(rigRobot.velocity.x);
        sensor.AddObservation(rigRobot.velocity.z);
    }

    /// <summary>
    /// 動作、控制機器人與回饋
    /// </summary>
   
    public override void OnActionReceived(float[] vectorAction)
    {
        //使用參數控制機器人
        Vector3 control= Vector3.zero  ;//控制軸向，指定成0
        control.x = vectorAction[0];
        control.z = vectorAction[1];//接收到的第一筆資料
        rigRobot.AddForce(control * speed);//剛體 添加推力


        //回饋方式
        //球進入球門=成功，加一分並結束
        if(BALL.complete)
        {
            SetReward(1);
            EndEpisode();
        }

        //機器人或足球掉到地板下=失敗，扣一分並結束
        if(transform.position.y<0 ||rigBall.position.y<0)
        {
            SetReward(-1);
            EndEpisode();
        }
    
    }


    /// <summary>
    /// 讓開發者測試環境
    /// </summary>
    /// <returns></returns>
    public override float[] Heuristic()
    {
        //提供開發者控制左右前後
        var action = new float[2];
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        return action;
    }
}
