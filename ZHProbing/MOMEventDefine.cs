using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class MomDefine
{
    public const string 三点定一平面 = "probe_three_point_plane";

    public const string 台阶面 = "probe_tow_point_height";

    public const string 两点凹槽 = "probe_tow_point_slot";

    public const string 两点凸台 = "probe_tow_point_rib";

    public const string 两点找正 = "probe_tow_point_angle_correction";

    public const string 两孔找正 = "probe_tow_hole_angle_correction";

    public const string 单向 = "probe_single_direction_point";

    public const string 三点序前外圆 = "probe_three_point_boss";

    public const string 三点单孔 = "probe_three_point_bore";

    public const string 外圆找正 = "probe_four_point_boss";

    public const string 单孔找正 = "probe_four_point_bore";


    public const string 测量类型 = "probe_result";

    public const string 类型选择 = "part_probe_type";

    public const string 指定坐标系 = "fixture_offset_value";

    public const string 补偿刀具编号 = "tool_length_adjust_register";

    public const string 外圆直径 = "probing_diameter";

    public const string 测量行程 = "probe_S_FA"; //1.01

    public const string 测量结果的置信区域 = "probe_S_TSA"; //1.01

    public const string 初始角度 = "probe_S_STA1";

    public const string 测量点X = "pos(0)";

    public const string 测量点Y = "pos(1)";

    public const string 测量点Z = "pos(2)";


    public const string 中心横坐标 = "probe_S_CPA";

    public const string 中心纵坐标 = "probe_S_CPO";



    public const string 刀具名称 = "tool_name";

    public const string 测头长度 = "probe_tool_length";

    public const string 部件侧面余量 = "probe_side_allowance";

    public const string 部件底面余量 = "probe_bottom_allowance";

    public const string 尺寸差异检查 = "probe_S_TDIF";

    public const string 上公差 = "probe_S_TUL";

    public const string 下公差 = "probe_S_TLL";

    public const string 设置坐标系偏置坐标X = "probe_S_XM";

    public const string 设置坐标系偏置坐标Y = "probe_S_YM";

    public const string 设置坐标系偏置坐标Z = "probe_S_ZM";

    public const string 测量结果输出 = "probe_result_output";

    public const string 测量点数 = "hole_probe_number";

    public const string 增量角 = "probe_S_INCA";

    public const string 测量方向 = "probe_direction"; //XAXIS YAXIS ZAXIS

    public const string 探测触碰点0 = "probe_contact_pos(0)";

    public const string 探测触碰点1 = "probe_contact_pos(1)";

    public const string 探测触碰点2 = "probe_contact_pos(2)";

    public const string 台阶面高度名义值 = "probe_height";

    public const string 在平面第1轴上 = "probe_S_SETV0";//（在平面第1轴上（横坐标）测量点P1和P2之间的距离）

    public const string 在平面第2轴上 = "probe_S_SETV2";//（在平面第1轴上（横坐标）测量点P1和P3之间的距离）

    public const string 在平面第3轴上 = "probe_S_SETV3";//（在平面第2轴上（纵坐标）测量点P1和P3 之间的距离）

    public const string ToolOffsetName = "probe_tool_name";
}

