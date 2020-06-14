1. Проекта представлява университет - Български университет.
   Има loggin и register - един user и един администратор.
   User има достъп само до details, а администратора може да създава, да променя и да трие.

2. Проекта съдържа една страница About - показва датата, в която учениците са се записали и колко на брой студенти има за тази дата;
   Втора страница Students - показва имената на студентите и датата в която са се записали; Трета страница Courses - курсовете и от кой бранш са;
   Четвърта страница Instructors - преподавателите, кога са назначени, къде им е офиса и кой курс преподават; И петта страница Departments -
   името на бранша, бюджета, дата от когато е бранша и кой отговаря за бранша.

3. Следователно разполагам с 5 Controllers - HomeController, StudentsController, CoursesController, InstructorsController и DepartmentsController
   като всеки от тях наследява Controller.

4. Имам папка Data - клас - ApplicationDbContext наследяващ IdentityDbContext и клас с инициализация на базата от данни. Папка с миграции. Папка Models - вътре са
   класовете - Course, CourseAssignment, Department, Enrollment, Instructor, OfficeAssignment, Student.

5. И имам папка View - вътре са папките Students, Courses, Departments, Instructors и Home, всеки от тях с по 5 - Create, Delete, Details, Edit
   и Index.

6. При страницата Instructors има опция Select, която при натискането отваря информация за курса преподаван от следния инструктор и неговият бранш.
   Там има друга опция Select, която пък отваря информация за студентите записали се в този курс и оценката, която имат.
