export interface PasswordUpdateDto {
    id: string | undefined;
    password: string;
    oldPassword: string | undefined;
    idLink: string | undefined;
}
