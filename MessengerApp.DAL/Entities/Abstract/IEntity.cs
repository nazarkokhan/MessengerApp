﻿namespace MessengerApp.DAL.Entities.Abstract
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }
}