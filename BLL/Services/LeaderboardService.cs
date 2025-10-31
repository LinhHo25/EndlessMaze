using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Services
{
    // Đã thay đổi từ 'internal' thành 'public'
    public class LeaderboardService
    {
        private ContextDB db = new ContextDB();

        /// <summary>
        /// Lấy Top 10 bảng xếp hạng Adventure (sắp xếp theo thời gian hoàn thành nhanh nhất).
        /// </summary>
        public List<LeaderboardAdventure> GetAdventureLeaderboard(int topN = 10)
        {
            try
            {
                return db.LeaderboardAdventure
                         .OrderBy(score => score.CompletionTimeSeconds) // Sắp xếp theo thời gian (ít là tốt)
                         .Take(topN) // Lấy 10
                         .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<LeaderboardAdventure>();
            }
        }

        /// <summary>
        /// Lấy Top 10 bảng xếp hạng Exploration (sắp xếp theo vật phẩm thu thập nhiều nhất).
        /// </summary>
        public List<LeaderboardExploration> GetExplorationLeaderboard(int topN = 10)
        {
            try
            {
                return db.LeaderboardExploration
                         .OrderByDescending(score => score.ItemsCollected) // Sắp xếp theo vật phẩm (nhiều là tốt)
                         .Take(topN)
                         .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<LeaderboardExploration>();
            }
        }

        /// <summary>
        /// Thêm điểm mới vào bảng xếp hạng Adventure.
        /// </summary>
        public bool AddAdventureScore(int userId, string username, int completionTime, int monstersKilled, int mapsCompleted)
        {
            try
            {
                LeaderboardAdventure newScore = new LeaderboardAdventure
                {
                    UserID = userId,
                    Username = username,
                    CompletionTimeSeconds = completionTime,
                    MonstersKilled = monstersKilled,
                    MapsCompleted = mapsCompleted,
                    AchievedDate = DateTime.Now
                };

                db.LeaderboardAdventure.Add(newScore);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
