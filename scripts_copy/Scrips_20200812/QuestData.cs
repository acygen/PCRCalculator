using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCRCaculator
{
    [System.Serializable]
    public class QuestData
    {
        public int quest_id;
        public int area_id;
        public string quest_name;
        public int limit_team_level;
        public int position_x;
        public int position_y;
        public int icon_id;
        public int stamina;
        public int stamina_start;
        public int team_exp;
        public int unit_exp;
        public int love;
        public int limit_time;
        public int daily_limit;
        public int clear_reward_group;
        public int rank_reward_group;
        public int wave_group_id_1;
        public int wave_group_id_2;
        public int wave_group_id_3;
        public int enemy_image_1;
        public int enemy_image_2;
        public int enemy_image_3;
        public int enemy_image_4;
        public int enemy_image_5;
        public int reward_image_1;
        public int reward_image_2;
        public int reward_image_3;
        public int reward_image_4;
        public int reward_image_5;

        public QuestData() { }
        public QuestData(int quest_id, int area_id, string quest_name, int limit_team_level, int position_x, 
            int position_y, int icon_id, int stamina, int stamina_start, int team_exp, int unit_exp, int love, 
            int limit_time, int daily_limit, int clear_reward_group, int rank_reward_group, int wave_group_id_1, 
            int wave_group_id_2, int wave_group_id_3, int enemy_image_1, int enemy_image_2, int enemy_image_3, 
            int enemy_image_4, int enemy_image_5, int reward_image_1, int reward_image_2, int reward_image_3, int reward_image_4, int reward_image_5)
        {
            this.quest_id = quest_id;
            this.area_id = area_id;
            this.quest_name = quest_name;
            this.limit_team_level = limit_team_level;
            this.position_x = position_x;
            this.position_y = position_y;
            this.icon_id = icon_id;
            this.stamina = stamina;
            this.stamina_start = stamina_start;
            this.team_exp = team_exp;
            this.unit_exp = unit_exp;
            this.love = love;
            this.limit_time = limit_time;
            this.daily_limit = daily_limit;
            this.clear_reward_group = clear_reward_group;
            this.rank_reward_group = rank_reward_group;
            this.wave_group_id_1 = wave_group_id_1;
            this.wave_group_id_2 = wave_group_id_2;
            this.wave_group_id_3 = wave_group_id_3;
            this.enemy_image_1 = enemy_image_1;
            this.enemy_image_2 = enemy_image_2;
            this.enemy_image_3 = enemy_image_3;
            this.enemy_image_4 = enemy_image_4;
            this.enemy_image_5 = enemy_image_5;
            this.reward_image_1 = reward_image_1;
            this.reward_image_2 = reward_image_2;
            this.reward_image_3 = reward_image_3;
            this.reward_image_4 = reward_image_4;
            this.reward_image_5 = reward_image_5;
        }
    }
    [System.Serializable]
    public class QuestRewardData
    {
        public int quest_id;
        public int area_id;
        public bool isHard;
        public string quest_name;
        public List<int> rewardEquips = new List<int>();
        public List<int> odds = new List<int>();
    }
    [System.Serializable]
    public class WaveGroupData
    {
        public int id;
        public int wave_group_id;
        public int odds;
        public List<EnemyDropData> enemyDropDatas;

        public WaveGroupData() { }
        public WaveGroupData(int id, int wave_group_id, int odds, List<EnemyDropData> enemyDropDatas)
        {
            this.id = id;
            this.wave_group_id = wave_group_id;
            this.odds = odds;
            this.enemyDropDatas = enemyDropDatas;
        }

        [System.Serializable]
        public class EnemyDropData
        {
            public int enemy_id;
            public int drop_gold;
            public int drop_reward_id;

            public EnemyDropData() { }
            public EnemyDropData(int enemy_id, int drop_gold, int drop_reward_id)
            {
                this.enemy_id = enemy_id;
                this.drop_gold = drop_gold;
                this.drop_reward_id = drop_reward_id;
            }
        }
    }
    [System.Serializable]
    public class EnemyRewardData
    {
        public int drop_reward_id;
        public int drop_count;
        public List<RewardData> rewardDatas;
        public EnemyRewardData() { }
        public EnemyRewardData(int drop_reward_id, int drop_count, List<RewardData> rewardDatas)
        {
            this.drop_reward_id = drop_reward_id;
            this.drop_count = drop_count;
            this.rewardDatas = rewardDatas;
        }

        [System.Serializable]
        public class RewardData
        {
            public int reward_type;
            public int reward_id;
            public int reward_num;
            public int odds;
            public RewardData() { }
            public RewardData(int reward_type, int reward_id, int reward_num,int odds)
            {
                this.reward_type = reward_type;
                this.reward_id = reward_id;
                this.reward_num = reward_num;
                this.odds = odds;
            }
        }
    }
    [System.Serializable]
    public class EquipmentCraft
    {
        public int equipment_id;
        public int crafted_cost;
        public List<int> conditon_equipment_id;
        public List<int> consum_nums;        

        public EquipmentCraft() { }
        public EquipmentCraft(int equipment_id, int crafted_cost, List<int> conditon_equipment_id, List<int> consum_nums)
        {
            this.equipment_id = equipment_id;
            this.crafted_cost = crafted_cost;
            this.conditon_equipment_id = conditon_equipment_id;
            this.consum_nums = consum_nums;
        }
        public void GetCraftIdTotal(Dictionary<int,EquipmentCraft> dic,out List<int> eqid,out List<int> num,out int totalcost,int count)
        {
            eqid = conditon_equipment_id;
            num = consum_nums;
            totalcost = crafted_cost;
            for(int n=0;n<num.Count;n++)
            {
                num[n] *= count;
            }
            for(int i = 0; i < conditon_equipment_id.Count; i++)
            {
                if (dic.TryGetValue(conditon_equipment_id[i],out EquipmentCraft value))
                {
                    eqid.RemoveAt(i);
                    int num_0 = num[i];
                    num.RemoveAt(i);
                    value.GetCraftIdTotal(dic, out List<int> eqid_1, out List<int> num_1,out int totalcost_1,num_0);
                    eqid.AddRange(eqid_1);
                    num.AddRange(num_1);
                    totalcost += totalcost_1;
                }
            }
        }
    }
    [System.Serializable]
    public class EquipmentGet
    {
        public int equipment_id;
        public bool isCraft;
        public int first_appear_quest_id; 
        public List<GetWay> getWays_n = new List<GetWay>();
        public List<GetWay> getWays_h = new List<GetWay>();
        public void GetBestWays(bool ignoreH, out List<int> ways,out List<int> odds)
        {
            ways = new List<int>();
            odds = new List<int>();
            getWays_n.Sort((a, b) => b.get_odds - a.get_odds);
            foreach(GetWay getWay in getWays_n)
            {
                ways.Add(getWay.get_quest_id);
                odds.Add(getWay.get_odds);
            }
        }
        [System.Serializable]
        public class GetWay
        {
            public int get_quest_id;
            public int get_area_id;
            public int get_odds;

            public GetWay() { }
            public GetWay(int get_quest_id, int get_area_id,int get_odds)
            {
                this.get_quest_id = get_quest_id;
                this.get_area_id = get_area_id;
                this.get_odds = get_odds;
            }
        }

    }
    [System.Serializable]
    public class CalcDics
    {
        //public Dictionary<int, QuestData> questDataDic = new Dictionary<int, QuestData>();//普通地图数据
        //public Dictionary<int, WaveGroupData> waveGroupDataDic = new Dictionary<int, WaveGroupData>();//怪物波次数据
        //public Dictionary<int, EnemyRewardData> enemyRewardDataDic = new Dictionary<int, EnemyRewardData>();//怪物掉落数据
        public Dictionary<int, EquipmentCraft> equipmentCraftDic = new Dictionary<int, EquipmentCraft>();//装备合成数据
        public Dictionary<int, QuestRewardData> questRewardDic = new Dictionary<int, QuestRewardData>();//地图掉落数据
        public Dictionary<int, EquipmentGet> equipmentGetDic = new Dictionary<int, EquipmentGet>();//装备获得数据
        public List<int> exp_cost;
        public List<int> skill_cost;

    }
}