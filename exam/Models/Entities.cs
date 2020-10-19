using System;
using System.Collections.Generic;

namespace exam.Models
{
    //Залы
    public class Hall
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
    }
    //Типы занятий
    public class TType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    //Тренировка
    public class Training
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        //Тренер
        public string TrainerId { get; set; }
        public AppUser Trainer { get; set; }

        //Тип
        public int TTypeId { get; set; }
        public TType TType { get; set; }

        //Зал
        public int HallId { get; set; }
        public Hall Hall { get; set; }

        public List<ClientTraining> Clients { get; set; }
        public Training ()
        {
            Clients = new List<ClientTraining>();
        }
    }

    //Связи
    public class ClientTraining
    {
        public int TrainingId { get; set; }
        public Training Training { get; set; }

        public string ClientId { get; set; }
        public AppUser Client { get; set; }
    }
}
