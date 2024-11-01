# frontend/Dockerfile

FROM node:20-alpine AS build

# 작업 디렉토리 설정
WORKDIR /app

# 타임존 설정
RUN ln -sf /usr/share/zoneinfo/Asia/Seoul /etc/localtime

# package.json 과 package-lock.json 파일 복사
COPY package*.json ./

# 의존성 설치
RUN npm install

# 전체 소스 복사 후 빌드
COPY . .
RUN npm run build

# Production 단계: Nginx를 활용해 정적 파일 서빙
FROM nginx:stable-alpine

# Nginx 설정 파일 복사
COPY nginx.conf /etc/nginx/nginx.conf

# 빌드 결과물을 Nginx 정적 파일 디렉토리로 복사
COPY --from=build /app/build /usr/share/nginx/html

# Nginx 포트 개방
EXPOSE 80

# Nginx 실행
CMD ["nginx", "-g", "daemon off;"]
