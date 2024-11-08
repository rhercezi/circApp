import { observer } from "mobx-react-lite";
import { useStore } from "../../stores/store";
import { useEffect, useState } from "react";
import { Checkbox, FormControl, FormControlLabel, IconButton, InputLabel, MenuItem, Select, Tooltip } from "@mui/material";
import { CircleDto } from "../../api/dtos/circle_dtos/CircleDto";
import Loader from "../common/Loader";
import ControlPointIcon from '@mui/icons-material/ControlPoint';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import AddTaskIcon from '@mui/icons-material/AddTask';
import { useNavigate, useParams } from "react-router-dom";
import { TaskDto } from "../../api/dtos/task_dtos/TaskDto";
import { getTimeSpan } from "../../helpers/helpers";

const TaskOverview = () => {
    const taskId = useParams<{ id: string }>();
    const { taskStore, userStore, circleStore } = useStore();
    const [tasks, setTasks] = useState(taskStore.tasks);
    const [circles, setCircles] = useState<CircleDto[]>([...circleStore.circlesMap.values()]);
    const [selectedCircle, setSelectedCircle] = useState<string>('search_by_user');
    const [includeCompleted, setIncludeCompleted] = useState<boolean>(true);
    const navigate = useNavigate();
    const userId = userStore.user?.id;

    useEffect(() => {
        setTasks(taskStore.tasks);
    }, [taskStore.tasks]);

    useEffect(() => {
        if (selectedCircle === 'search_by_user') {
            taskStore.getTasks(userId!, includeCompleted, false);
        } else if (selectedCircle === 'all_circles') {
            taskStore.getTasks(userId!, includeCompleted, true);
        } else if (selectedCircle) {
            taskStore.getTasks(selectedCircle, includeCompleted);
        }
    }, [selectedCircle, includeCompleted]);

    useEffect(() => {
        setCircles([...circleStore.circlesMap.values()]);
    }, [circleStore, circleStore.circlesMap]);

    const newTask = () => {
        navigate('/tasks/new');
    }

    const editTask = (taskId: string) => {
        navigate(`/tasks/edit/${taskId}`);
    }

    const deleteTask = (taskId: string) => {
        taskStore.deleteTask(taskId);
    }

    const calculateCompleteness = (task: TaskDto) => {
        if (task.isCompleted) {
            return 100;
        } else if (task.userModels && task.userModels.length > 0) {
            let completed = 0;
            for (let user of task.userModels) {
                if (user.isCompleted) {
                    completed++;
                }
            }
            return completed / task.userModels.length * 100;
        } else {
            return 0;
        }
    }

    if (taskStore.loading) {
        return (<Loader text={taskStore.loaderText}></Loader>)
    }

    function completeTask(id: string) {
        const task = taskStore.tasks.find(task => task.id === id);
        if (task) {
            if (task.userModels && task.userModels.length > 0) {
                if (userId) {
                    taskStore.completeTask({
                        taskId: id,
                        userId: userId,
                        circleId: undefined
                    });
                }
            } else {
                taskStore.completeTask({
                    taskId: id,
                    circleId: task.circleId,
                    userId: undefined
                });
            }
        }
    }

    function userCanComplete(task: TaskDto): boolean {
        if (selectedCircle === 'search_by_user'
            && task.userModels
            && !task.userModels.find(user => user.id === userId)
            && task.ownerId !== userId
        ) {
            return false;
        } else {
            return true;
        }
    }

    return (
        <>
            {
                !taskId?.id && (
                    <>
                        <div className="task-overview-header">
                            <FormControl variant="outlined" sx={{ minWidth: 200 }} >
                                <InputLabel id="circle-select">Select Circle</InputLabel>
                                <Select
                                    id="circle-select"
                                    labelId="circle-select"
                                    label="Select Circle"
                                    value={selectedCircle}
                                    onChange={(e) => {
                                        setSelectedCircle(e.target.value);
                                    }}
                                >
                                    <MenuItem selected={true} key="search_by_user" value={'search_by_user'}>By User</MenuItem>
                                    <MenuItem key="all_circles" value={'all_circles'}>All Circles</MenuItem>
                                    {
                                        circles.map(circle => (
                                            <MenuItem key={circle.id} value={circle.id}>{circle.name}</MenuItem>
                                        ))
                                    }
                                </Select>
                            </FormControl>
                            <Tooltip title="Create Task">
                                <IconButton aria-label="create" size="large" onClick={newTask}>
                                    <ControlPointIcon fontSize="large" />
                                </IconButton>
                            </Tooltip>
                        </div>
                        <div className="task-overview-header">
                            <FormControlLabel
                                control={<Checkbox
                                    checked={includeCompleted}
                                    onChange={(e) => {
                                        setIncludeCompleted(e.target.checked);
                                    }} />}
                                label="Include Completed" />
                        </div>
                    </>
                )
            }
            <div className="day-overview-items-container">
                {
                    tasks.map(task => (
                        <div key={task.id} className="task-overview-item-wrapper">
                            <div className="task-overview-item">
                                <div className="task-overview-details">
                                    <h3>{task.title}</h3>
                                    <p>{task.description}</p>
                                    <p>
                                        <strong>Due Date: </strong>
                                        {new Date(task.endDate).toLocaleString()}
                                    </p>
                                    <p>
                                        <strong>{new Date() < new Date(task.endDate)
                                            ? 'Time Remaining: '
                                            : 'Overdue: '}</strong>
                                        {getTimeSpan(new Date(), new Date(task.endDate))}
                                    </p>
                                    {task.userModels && task.userModels.length > 0 &&
                                        <p>
                                            <strong>Completeness by user: </strong>
                                            {
                                                task.userModels.map(user => (
                                                    <span key={user.id}>
                                                        <br />
                                                        <span key={user.id}>
                                                            <strong>{user.userName}: </strong>
                                                            {user.isCompleted ? ' (completed)' : ' (uncompleted)'}
                                                        </span>
                                                    </span>
                                                ))
                                            }
                                        </p>
                                    }
                                </div>
                                <div className="task-overview-actions">
                                    <Tooltip title="Delete Task">
                                        <IconButton aria-label="delete" size="large" onClick={() => { deleteTask(task.id) }}
                                            disabled={task.ownerId !== userId}>
                                            <DeleteIcon fontSize="large" />
                                        </IconButton>
                                    </Tooltip>
                                    <Tooltip title="Edit Task">
                                        <IconButton aria-label="edit" size="large" onClick={() => { editTask(task.id) }}
                                            disabled={task.ownerId !== userId}>
                                            <EditIcon fontSize="large" />
                                        </IconButton>
                                    </Tooltip>
                                    <Tooltip title="Complete Task">
                                        <IconButton aria-label="complete" size="large" onClick={() => { completeTask(task.id) }}
                                            disabled={!userCanComplete(task)}>
                                            <AddTaskIcon fontSize="large" />
                                        </IconButton>
                                    </Tooltip>
                                </div>
                            </div>
                            <div className="completeness-div-outer">
                                <div className="completeness-div" style={{ width: `${calculateCompleteness(task)}%` }} ></div>
                            </div>
                        </div>
                    ))
                }
            </div>
        </>
    );
}

export default observer(TaskOverview);