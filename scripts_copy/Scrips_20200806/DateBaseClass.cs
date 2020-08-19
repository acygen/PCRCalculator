using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class equipment_data
{
    public int equipment_id;
    public string equipment_name;
    public string description;
    public int promotion_level;
    public int craft_flg;
    public int equipment_enhance_point;
    public int sale_price;
    public int require_level;
    public float hp;
    public float atk;
    public float magic_str;
    public float def;
    public float magic_def;
    public float physical_critical;
    public float magic_critical;
    public float wave_hp_recovery;
    public float wave_energy_recovery;
    public float dodge;
    public float physical_penetrate;
    public float magic_penetrate;
    public float life_steal;
    public float hp_recovery_rate;
    public float energy_recovery_rate;
    public float energy_reduce_rate;
    public int enable_donation;
    public float accuracy;
    public int display_item;
    public int item_type;

    public override string ToString()
    {
        return equipment_id + "-"+ equipment_name;
    }
}
