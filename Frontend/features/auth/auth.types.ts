import { UserRoles } from "@/features/users/users.types";

export namespace Login {
  export interface Request {
    username: string;
    password: string;
  }

  export interface Response {
    accessToken: string;
    refreshToken: string;
  }
}

export namespace Refresh {
  export interface Request { }
  export interface Response {
    accessToken: string;
    refreshToken: string;
  }
}

export namespace GetMe {
  export interface Refresh { };
  export interface Response {
    id: string;
    userName: string;
    locationId: string;
    locationName: string;
    isFirstLogin: boolean;
    roles: UserRoles[];
  }
}

export namespace FirstChangePassword {
  export interface Request {
    newPassword: string;
  }
  export interface Response { }
}


export namespace ChangePassword {
  export interface Request {
    oldPassword: string;
    newPassword: string;
  }

  export interface Response {
    message: string;
  }
}

export namespace Logout {
  export type Request = void;
  export type Response = void;
}
