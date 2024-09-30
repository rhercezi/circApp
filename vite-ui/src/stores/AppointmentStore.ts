import { makeAutoObservable, runInAction } from "mobx";
import { AppointmentDto, DetachedDetailsDto } from "../api/dtos/appointment_dtos/AppointmentDto";
import apiClient from "../api/apiClient";

export default class AppointmentStore {
    appointments: AppointmentDto[] = [];
    loading: boolean = false;
    loaderText: string = '';
    errorMap = new Map<string, string>();

    constructor() {
        makeAutoObservable(this);
    }

    getAppointment = async (id: string, userId: string): Promise<AppointmentDto | undefined> => {
        let appointment = this.appointments.find(appointment => appointment.id === id);
        if (!appointment) {
            appointment = await apiClient.Appointments.getAppointment(id, userId);
        }
        return appointment;
    }

    getAppointments = async (circleId: string | undefined, userId: string | undefined, dateFrom: Date, dateTo: Date) => {
        this.loaderText = 'Getting appointments...';
        this.loading = true;
        try {
            let data = circleId ? await apiClient.Appointments.getByCircle(circleId!, dateFrom, dateTo) : await apiClient.Appointments.getByUser(userId!, dateFrom, dateTo);
            runInAction(() => {
                this.appointments = data;
                this.errorMap.delete('getAppointments');
            });
        } catch (error: any) {
            if (error.response && error.response.status > 399 && error.response.status < 500) {
                runInAction(() => {
                    this.errorMap.set('getAppointments', error.response.data);
                });
            } else {
                runInAction(() => {
                    this.errorMap.set('getAppointments', 'Something went wrong, please try again later.');
                });
            }
        } finally {
            runInAction(() => {
                this.loading = false;
            });
        }
    }

    createAppointment = async (appointment: AppointmentDto) => {
        this.loaderText = 'Creating appointment...';
        this.loading = true;
        try {
            await apiClient.Appointments.create(appointment);
            runInAction(() => {
                this.errorMap.delete('createAppointment');
            });
        } catch (error: any) {
            if (error.response && error.response.status > 399 && error.response.status < 500) {
                runInAction(() => {
                    this.errorMap.set('createAppointment', error.response.data);
                });
            } else {
                runInAction(() => {
                    this.errorMap.set('createAppointment', 'Something went wrong, please try again later.');
                });
            }
        } finally {
            runInAction(() => {
                this.loading = false;
            });
        }
    }

    updateAppointment = async (id: string, jsonPatch: string, userId: string) => {
        this.loaderText = 'Updating appointment...';
        this.loading = true;
        try {
            let data = await apiClient.Appointments.update(id, jsonPatch, userId);
            runInAction(() => {
                this.appointments = this.appointments.map(appointment => appointment.id === id ? data.data : appointment);
                this.errorMap.delete('updateAppointment');
            });
        }
        catch (error: any) {
            if (error.response && error.response.status > 399 && error.response.status < 500) {
                runInAction(() => {
                    this.errorMap.set('updateAppointment', error.response.data);
                });
            } else {
                runInAction(() => {
                    this.errorMap.set('updateAppointment', 'Something went wrong, please try again later.');
                });
            }
        } finally {
            runInAction(() => {
                this.loading = false;
            });
        }
    }

    deleteAppointment = async (id: string, userId: string) => {
        this.loaderText = 'Deleting appointment...';
        this.loading = true;
        try {
            await apiClient.Appointments.delete(id, userId);
            runInAction(() => {
                this.appointments = this.appointments.filter(appointment => appointment.id !== id);
                this.errorMap.delete('deleteAppointment');
            });
        } catch (error: any) {
            if (error.response && error.response.status > 399 && error.response.status < 500) {
                runInAction(() => {
                    this.errorMap.set('deleteAppointment', error.response.data);
                });
            } else {
                runInAction(() => {
                    this.errorMap.set('deleteAppointment', 'Something went wrong, please try again later.');
                });
            }
        } finally {
            runInAction(() => {
                this.loading = false;
            });
        }
    }

    createDetails = async (detailsDto: DetachedDetailsDto) => {
        this.loaderText = 'Creating details...';
        this.loading = true;
        try {
            await apiClient.Appointments.createDetails(detailsDto);
            runInAction(() => {
                this.errorMap.delete('createDetails');
            });
        } catch (error: any) {
            if (error.response && error.response.status > 399 && error.response.status < 500) {
                runInAction(() => {
                    this.errorMap.set('createDetails', error.response.data);
                });
            } else {
                runInAction(() => {
                    this.errorMap.set('createDetails', 'Something went wrong, please try again later.');
                });
            }
        } finally {
            runInAction(() => {
                this.loading = false;
            });
        }
    }

    updateDetails = async (id: string, jsonPatch: string, userId: string) => {
        this.loaderText = 'Updating details...';
        this.loading = true;
        try {
            let data = await apiClient.Appointments.updateDetails(id, jsonPatch, userId);
            runInAction(() => {
                this.appointments = this.appointments.map(appointment => appointment.details?.appointmentId === id ? { ...appointment, details: data.data } : appointment);
                this.errorMap.delete('updateDetails');
            });
        } catch (error: any) {
            if (error.response && error.response.status > 399 && error.response.status < 500) {
                runInAction(() => {
                    this.errorMap.set('updateDetails', error.response.data);
                });
            } else {
                runInAction(() => {
                    this.errorMap.set('updateDetails', 'Something went wrong, please try again later.');
                });
            }
        } finally {
            runInAction(() => {
                this.loading = false;
            });
        }
    }

    deleteDetails = async (id: string, userId: string) => {
        this.loaderText = 'Deleting details...';
        this.loading = true;
        try {
            await apiClient.Appointments.deleteDetails(id, userId);
            runInAction(() => {
                this.appointments = this.appointments.map(appointment => appointment.details?.appointmentId === id ? { ...appointment, details: undefined } : appointment);
                this.errorMap.delete('deleteDetails');
            });
        } catch (error: any) {
            if (error.response && error.response.status > 399 && error.response.status < 500) {
                runInAction(() => {
                    this.errorMap.set('deleteDetails', error.response.data);
                });
            } else {
                runInAction(() => {
                    this.errorMap.set('deleteDetails', 'Something went wrong, please try again later.');
                });
            }
        } finally {
            runInAction(() => {
                this.loading = false;
            });
        }
    }
}