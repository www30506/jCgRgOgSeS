using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData {
	/*遊戲*/
	public static float TARGET_TIME = 30;//每個球給的時間;

	public static int ENDLESS_MODE_TARGET_NUMBER_START = 1;//無盡模式完成目標的起始數字
	public static int ENDLESS_MODE_TARGET_NUMBER_END = 6;//無盡模式完成目標的結束數字
	public static int ENDLESS_MODE_START_changeOperationCount = 9; //無盡模式起始的可變換次數
	public static float ENDLESS_MODE_START_TIME = 180; //無盡模式的起始時間
	public static float ENDLESS_MODE_COMPLETE_ADD_TIME = 30; //無盡模式完成目標增加的時間
	public static int ENDLESS_MODE_COMPLETE_TARGET_ADD_changeOperationCount = 10; //無盡模式完成目標補充的可變換次數
}