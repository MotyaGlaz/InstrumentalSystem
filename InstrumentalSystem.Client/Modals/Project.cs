using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Google.Protobuf.WellKnownTypes;

namespace InstrumentalSystem.Client.Modals
{
    //Сущность, описывающая проект в системе
    public class Project : INotifyPropertyChanged
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Status { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime LastModifiedDate { get; private set; }
        public string Path { get; private set; }
        public List<User> Users => new List<User>(_users);

        private List<User> _users;
        private string _usersInfo => string.Join(", ", Users);
        private ObservableCollection<CustomCollection> _comments;

        public Project(int id, string name, string status, DateTime createdDate, DateTime lastModifiedDate,
            ObservableCollection<CustomCollection> comments,
            List<User> users, string path)
        {
            Id = id;
            Name = name;
            Status = status;
            CreatedDate = createdDate;
            LastModifiedDate = lastModifiedDate;
            _users = users;
            Path = path;

            _comments = comments;
        }

        public ObservableCollection<CustomCollection> Comments
        {
            get { return _comments; }
            set
            {
                if (_comments != value)
                {
                    _comments = value;
                    OnPropertyChanged(nameof(Comments));
                }
            }
        }

        public void AddComment(string comment, int userId)
        {
            int commentNumber = Comments.Count + 1;

            bool isNew = true;
            
            var newComment = new CustomCollection(commentNumber, comment, userId, isNew);
            newComment.Username = Database.Instance.GetUserById(userId).Login;
            
            Comments.Add(newComment);
        }

        public string ProjectDetails
        {
            get
            {
                return
                    $"Название проекта: {Name}\nСтатус: {Status}\nДата создания: {CreatedDate}\nДата последнего изменения: {LastModifiedDate}\nУчастники проекта: {_usersInfo}";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CustomCollection
    {
        public int Key { get; private set; }
        public string Value { get; private set; }
        public int UserId { get; private set; }
        public string Username { get; set; }
        public bool IsNew { get; private set; }

        public CustomCollection(int key, string value, int userId, bool isNew)
        {
            Key = key;
            Value = value;
            UserId = userId;
            IsNew = isNew;
        }
    }
}