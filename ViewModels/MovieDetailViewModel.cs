using CinemaManagement.Models;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaManagement.ViewModels
{
    class MovieDetailViewModel
    {
        public ICollection<Ticket> ticketList { get; set; }
        public Movie movie { get; set; }
        public ShowTime ShowTime { get; set; }
        public List<string> rowList { get; set; }
        public int seatMapWidth { get; set; }
        public int seatMapHeight { get; set; }
        public MovieDetailViewModel()
        {
            ShowTime = new ShowTime()
            {
                ShowTimeId = 1,
                ShowDate = DateTime.Now,
                MaxRow = 8,
                MaxCol = 18,
                MovieId = 1,
                Movie = movie,
            };

            GenerateTicketList();
            ConvertRowListToStringList((int)ShowTime.MaxRow);
        }

        public void GenerateTicketList()
        {
            // Generate ticket list
            if (ticketList == null)
            {
                ticketList = new List<Ticket>();
            }
            else
            {
                ticketList.Clear();
            }

            for (int i = 0; i < ShowTime.MaxRow; i++)
            {
                for (int j = 0; j < ShowTime.MaxCol; j++)
                {
                    ticketList.Add(new Ticket()
                    {
                        IsAvailable = true,
                        Price = 100,
                        TicketId = 001,
                        Col = j,
                        Row = ConvertRowToString(i),
                    });
                }
            }

            seatMapWidth = (int)ShowTime.MaxCol * 50 + ((int)ShowTime.MaxCol - 1)*12;
            seatMapHeight = (int)ShowTime.MaxRow * 50 + ((int)ShowTime.MaxRow - 1) * 12;

            // Randomly set some tickets to unavailable
            Random random = new Random();
            for (int i = 0; i < 10; i++)
            {
                int randomRow = random.Next(0, (int)ShowTime.MaxRow);
                int randomCol = random.Next(0, (int)ShowTime.MaxCol);
                ticketList.ElementAt(randomRow * (int)ShowTime.MaxCol + randomCol).IsAvailable = false;
            }
        }

        public string ConvertRowToString(int row)
        {
            return ((char)('A' + row)).ToString();
        }

        public void ConvertRowListToStringList(int maxRow)
        {
            if (rowList == null)
            {
                rowList = new List<string>();
            }
            else
            {
                rowList.Clear();
            }

            for (int i=0; i<maxRow;i++)
            {
                rowList.Add(ConvertRowToString(i));
            }
        }

    }
    
}
