﻿namespace InterReact;

public enum OrderConditionType
{
    None = 0,
    Price = 1,
    // 2 is missing!
    Time = 3,
    Margin = 4,
    Execution = 5,
    Volume = 6,
    PercentChange = 7
}
