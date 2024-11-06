# frontend/Dockerfile

FROM nginx:stable-alpine

# 빌드 결과물을 Nginx 정적 파일 디렉토리로 복사
COPY dist /usr/share/nginx/html

# Nginx 포트 개방
EXPOSE 80

# Nginx 실행
CMD ["nginx", "-g", "daemon off;"]
