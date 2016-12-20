using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalStorage.Model
{
    public class TaskModel
    {
        /// <summary>
        /// Gets or sets to do identifier.
        /// </summary>
        /// <value>
        /// To do identifier.
        /// </value>
        public int Id { get; set; }  //rename as ToDoId and Add [Key] it's repair Delete *atalog
        public int RealId { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public virtual UserModel User { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this todo-item is completed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this todo-item is completed; otherwise, <c>false</c>.
        /// </value>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Gets or sets the name (description) of todo-item.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

    }
}
